using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public class SpatialHash
    {
        /* the square cell gridLength of the grid. Must be larger than the largest shape in the space. */
        private float gridHeightRes;
        private float gridWidthRes;
        private decimal invCellSize;

        public SpatialHash(int _width, int _height, int _cellSize)
        {
            gridWidthRes = _width;
            gridHeightRes = _height;

            cellSize = _cellSize;
            invCellSize = (decimal)1 / cellSize;

            gridWidth = (int)Math.Ceiling(_width * invCellSize);
            gridHeight = (int)Math.Ceiling(_height * invCellSize);

            gridLength = gridWidth * gridHeight;

            grid = new List<List<Body>>(gridLength);

            for (int i = 0; i < gridLength; i++)
                grid.Add(new List<Body>());

        }

        public void addBody(Body b)
        {
            addIndex(b, getIndex1DVec(clampToGridVec(b.position.Y, b.position.Y)));
        }

        public void removeBody(Body b)
        {
            removeIndexes(b);
        }

        public void updateBody(Body b)
        {
            updateIndexes(b, aabbToGrid(b.aabb.min, b.aabb.max));
        }

        public List<Body> getAllBodiesSharingCellsWithBody(Body body)
        {
            var collidingBodies = new List<Body>();
            foreach (int i in body.gridIndex)
            {
                if (grid[i].Count == 0)
                    continue;

                foreach (var cbd in grid[i].ToArray())
                {
                    if (cbd == body)
                        continue;
                    collidingBodies.Add(cbd);
                }
            }
            return collidingBodies;
        }

        public bool isBodySharingAnyCell(Body body)
        {
            foreach (int i in body.gridIndex)
            {
                if (grid[i].Count == 0)
                    continue;

                foreach (var cbd in grid[i].ToArray())
                {
                    if (cbd == body)
                        continue;
                    return true;
                }
            }
            return false;
        }

        public int getIndex1DVec(Vector2 _pos)
        {
            return (int)(Math.Floor(_pos.X * (float)invCellSize) + gridWidth * Math.Floor(_pos.Y * (float)invCellSize));
        }

        private int getIndex(float _pos)
        {
            return (int)(_pos * (float)invCellSize);
        }

        private int getIndex1D(int _x, int _y)
        {            
            return (int)(_x + gridWidth * _y);
        }

        private void updateIndexes(Body b, List<int> _ar)
        {
            foreach (int i in b.gridIndex)
            {
                removeIndex(b, i);
            }            
            b.gridIndex.Clear();

            foreach (int i in _ar)
            {
                addIndex(b, i);
            }
        }

        private void addIndex(Body b, int _cellPos)
        {
            grid[_cellPos].Add(b);
            b.gridIndex.Add(_cellPos);
        }
        private void removeIndexes(Body b) 
        {
            foreach (int i in b.gridIndex)
            {
                removeIndex(b, i);
            }            
            b.gridIndex.Clear();
        }
        private void removeIndex(Body b, int _pos) 
        {
            grid[_pos].Remove(b);
        }

        private bool isValidGridPos(int num)
        {
            if (num < 0 || num >= gridLength)
                return false;
            else
                return true;
        }

        public Vector2 clampToGridVec(float x, float y)
        {
            Vector2 _vec = new Vector2(x, y);
            _vec.X = MathHelper.Clamp(_vec.X, 0, gridWidthRes - 1);
            _vec.Y = MathHelper.Clamp(_vec.Y, 0, gridHeightRes - 1);
            return _vec;
        }

        private List<int> aabbToGrid(Vector2 _min, Vector2 _max)
        {
            var arr = new List<int>();
            int aabbMinX = MathHelper.Clamp(getIndex(_min.X), 0, gridWidth - 1);
            int aabbMinY = MathHelper.Clamp(getIndex(_min.Y), 0, gridHeight - 1);
            int aabbMaxX = MathHelper.Clamp(getIndex(_max.X), 0, gridWidth - 1);
            int aabbMaxY = MathHelper.Clamp(getIndex(_max.Y), 0, gridHeight - 1);

            int aabbMin = getIndex1D(aabbMinX, aabbMinY);
            int aabbMax = getIndex1D(aabbMaxX, aabbMaxY);

            arr.Add(aabbMin);
            if (aabbMin != aabbMax)
            {
                arr.Add(aabbMax);
                int lenX = aabbMaxX - aabbMinX + 1;
                int lenY = aabbMaxY - aabbMinY + 1;
                for (int x = 0; x < lenX; x++)
                {
                    for (int y = 0; y < lenY; y++)
                    {
                        if ((x == 0 && y == 0) || (x == lenX - 1 && y == lenY - 1))
                            continue;
                        arr.Add(getIndex1D(x, y) + aabbMin);
                    }
                }
            }
            return arr;
        }

        /* DDA line algorithm. @author playchilla.com */
        public List<int> lineToGrid(float x1, float y1, float x2, float y2)
        {
            var arr = new List<int>();

            int gridPosX = getIndex(x1);
            int gridPosY = getIndex(y1);

            if (!isValidGridPos(gridPosX) || !isValidGridPos(gridPosY))
                return arr;

            arr.Add(getIndex1D(gridPosX, gridPosY));

            float dirX = x2 - x1;
            float dirY = y2 - y1;
            float distSqr = dirX * dirX + dirY * dirY;
            if (distSqr < VectorMath.EPSILON)
                return arr;

            float nf = (float)(1 / Math.Sqrt(distSqr));
            dirX *= nf;
            dirY *= nf;

            float deltaX = cellSize / Math.Abs(dirX);
            float deltaY = cellSize / Math.Abs(dirY);

            float maxX = gridPosX * cellSize - x1;
            float maxY = gridPosY * cellSize - y1;
            if (dirX >= 0)
                maxX += cellSize;
            if (dirY >= 0)
                maxY += cellSize;
            maxX /= dirX;
            maxY /= dirY;

            int stepX = Math.Sign(dirX);
            int stepY = Math.Sign(dirY);
            int gridGoalX = getIndex(x2);
            int gridGoalY = getIndex(y2);
            int currentDirX = gridGoalX - gridPosX;
            int currentDirY = gridGoalY - gridPosY;

            while (currentDirX * stepX > 0 || currentDirY * stepY > 0)
            {
                if (maxX < maxY)
                {
                    maxX += deltaX;
                    gridPosX += stepX;
                    currentDirX = gridGoalX - gridPosX;
                }
                else
                {
                    maxY += deltaY;
                    gridPosY += stepY;
                    currentDirY = gridGoalY - gridPosY;
                }

                if (!isValidGridPos(gridPosX) || !isValidGridPos(gridPosY))
                    break;

                arr.Add(getIndex1D(gridPosX, gridPosY));
            }
            return arr;
        }

        public void clear()
        {
            foreach (var cell in grid)
            {
                if (cell.Count > 0)
                {
                    foreach (var co in cell)
                    {
                        co.gridIndex.Clear();
                    }
                    cell.Clear();
                }
            }
        }

        public int cellSize { get; set; }

        /* the world space width */
        public int gridWidth { get; set; }

        /* the world space height */
        public int gridHeight { get; set; }

        /* the number of buckets (i.e. cells) in the spatial grid */
        public int gridLength { get; set; }
        /* the array-list holding the spatial grid buckets */
        public List<List<Body>> grid { get; set; }
    }
}
