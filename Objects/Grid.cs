﻿using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using Match3.Misc;
using Match3.Rooms;
using Match3.Animation;

namespace Match3.Objects
{
    public class Grid : GameObject, IHasRoomAccess, IMouseListener
    {
        #region Events

        public event Action<MouseButtonEventArgs> OnMouseDown;
        public event Action<MouseButtonEventArgs> OnMouseUp;
        public event Action<int> OnMatchCollected;
        public event Action OnGridChanged;

        #endregion

        #region Fields

        private Tile[,] grid;
        private Sprite[,] floor;
        private bool[,] matches;
        private int matchCount;

        #endregion

        #region Properties

        public float X { get; private set; }
        public float Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int CellWidth { get; private set; } = 48;
        public int CellHeight { get; private set; } = 48;
        public int TweenCount { get; private set; } = 0;
        public bool IsBlocked { get; private set; }
        public Tuple<int, int> Selected { get; private set; }
        public Tuple<int, int> SelectedByClick { get; private set; }
        
        public Tile ClickedTile => SelectedByClick == null ? null : grid[SelectedByClick.Item1, SelectedByClick.Item2];
        public bool IsSelected => Selected != null;
        public int RealWidth => Width * CellWidth;
        public int RealHeight => Height * CellHeight;

        #endregion

        public Grid(float x, float y, int width, int height)
        {
            grid = new Tile[width, height];
            floor = new Sprite[width, height];
            matches = new bool[width, height];
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }

        #region Callbacks

        public void Created(Room room, LinkedListNode<GameObject> node)
        {
            var sheet = ResourceManager.GetSpritesheetByTileId(RandomTile());
            CellWidth = sheet.TileWidth;
            CellHeight = sheet.TileHeight;

            // Init grid and floor
            var floorTexture = ResourceManager.LoadTexture("cell", "Assets/Grid/grid");
            var x = 0f;
            var y = 0f;
            var ox = (CellWidth - floorTexture.Size.X) / 2;
            var oy = CellHeight / 2;
            for (int cellX = 0; cellX < Width; ++cellX) {
                x = X + CellWidth * cellX;
                for (int cellY = 0; cellY < Height; ++cellY) {
                    y = Y + CellHeight * cellY;

                    floor[cellX, cellY] = new Sprite(floorTexture) {
                        Position = new Vector2f(x + ox, y + oy)
                    };

                    var tile = new Tile(RandomTile(), x, y);
                    grid[cellX, cellY] = tile;
                    room.Add(tile);
                }
            }

            // Generate valid grid
            RandomizeField();
            room.OnEnter += RandomizeField;
            room.OnLeave += () => {
                Selected = null;
                SelectedByClick = null;
            };
        }

        public void GridChanged()
        {
            OnGridChanged?.Invoke();
            if (matchCount > 0) {
                OnMatchCollected?.Invoke(matchCount);
            }

            var group = new TweenGroup();
            OnUpdate += group.Update;
            group.OnFinished += () => {
                OnUpdate -= group.Update;
                FindMatches();
                if (matchCount > 0) {
                    GridChanged();
                } else {
                    IsBlocked = false;
                }
            };
            
            // Stuff below controls game logic => bad coding, needs some refactor
            int shift = 0;
            var room = RoomManager.CurrentRoom as GameRoom;
            var target = new Vector2f(room.boss.Position.X + room.boss.Width / 2, room.boss.Position.Y);
            for (int x = 0; x < Width; ++x) {
                shift = 0;
                for (int y = Height - 1; y >= 0; --y) {
                    if (!matches[x, y]) {
                        if (shift > 0) {
                            group.Add( grid[x, y].MoveTo(grid[x, y + shift].Position) );
                            grid[x, y + shift].MoveTo(grid[x, y].Position, true);
                            Helpers.Swap(ref grid[x, y], ref grid[x, y + shift]);
                        }
                        continue;
                    }

                    // TODO: create "destroy" event for tiles and attach illusions' creation to it from GameRoom
                    var cell = grid[x, y];
                    var damage = matchCount / 3f;
                    var obj = room.Add<Projectile>(cell.Type, cell.Position, target).Value as Projectile;
                    obj.OnDestroy += () => {
                        room.boss.Damage(damage);

                        // using hardcoded strings == bad
                        SoundManager.PlaySound("attack" + GameManager.Rand.Next(1, 14), 90f, true);
                        SoundManager.PlaySound("monster" + GameManager.Rand.Next(1, 4), 50f, true);
                    };

                    cell.Type = RandomTile();
                    ++shift;
                }

                for (int i = 0; i < shift; ++i) {
                    var pos = new Vector2f(grid[x, i].X, grid[x, i].Y);
                    grid[x, i].MoveTo(new Vector2f(pos.X, Y - (shift - i) * CellHeight), true);
                    group.Add( grid[x, i].MoveTo(pos) );
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            for (int x = 0; x < Width; ++x) {
                for (int y = 0; y < Height; ++y) {
                    GameManager.Window.Draw(floor[x, y]);
                }
            }
        }

        public void MouseDown(MouseButtonEventArgs e)
        {
            OnMouseDown?.Invoke(e);
            if (IsBlocked || e.Button != Mouse.Button.Left || !IsInGrid(e.X, e.Y)) {
                return;
            }

            var cellX = (int) Math.Truncate((e.X - X) / CellWidth);
            var cellY = (int) Math.Truncate((e.Y - Y) / CellHeight);
            Selected = new Tuple<int, int>(cellX, cellY);
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
            OnMouseUp?.Invoke(e);
            if (IsBlocked || e.Button != Mouse.Button.Left || !IsSelected) {
                return;
            }

            var cellX = (int) Math.Truncate((e.X - X) / CellWidth);
            var cellY = (int) Math.Truncate((e.Y - Y) / CellHeight);

            if (SelectedByClick != null && IsAdjacent(cellX, cellY, SelectedByClick.Item1, SelectedByClick.Item2)) {
                Selected = SelectedByClick;
            }

            if (cellX == Selected.Item1 && cellY == Selected.Item2) {
                //grid[cellX, cellY].PlayRandom();
                SelectedByClick = Selected;
                return;
            }

            var direction = new Vector2i(cellX - Selected.Item1, cellY - Selected.Item2);
            if (SelectedByClick != null && Math.Abs(direction.X) + Math.Abs(direction.Y) > 1) {
                return;
            }

            var newX = Selected.Item1 + Math.Sign(direction.X);
            var newY = Selected.Item2 + Math.Sign(direction.Y);

            if (Math.Abs(direction.X) > Math.Abs(direction.Y) && newX >= 0 && newX < Width) {
                Swap(newX, Selected.Item2, Selected.Item1, Selected.Item2);
            } else if (newY >= 0 && newY < Height) {
                Swap(Selected.Item1, newY, Selected.Item1, Selected.Item2);
            }

            Selected = null;
            SelectedByClick = null;
        }

        #endregion

        #region Utils

        public void RandomizeField()
        {
            Func<int, int, int, bool> hasLeftMatch =
                (id, x, y) => x > 1 && grid[x - 1, y].Type == id && grid[x - 2, y].Type == id;
            Func<int, int, int, bool> hasTopMatch =
                (id, x, y) => y > 1 && grid[x, y - 1].Type == id && grid[x, y - 2].Type == id;

            if (ResourceManager.Tiles.Count < 2) {
                throw new InvalidOperationException("Too few tiles for generating valid grid");
            }
            
            var sheet = ResourceManager.GetSpritesheetByTileId(RandomTile());
            CellWidth = sheet.TileWidth;
            CellHeight = sheet.TileHeight;

            for (int cellX = 0; cellX < Width; ++cellX) {
                for (int cellY = 0; cellY < Height; ++cellY) {
                    int tileId;
                    do {
                        tileId = RandomTile();
                    } while (hasLeftMatch(tileId, cellX, cellY) || hasTopMatch(tileId, cellX, cellY));
                    grid[cellX, cellY].Type = tileId;
                    grid[cellX, cellY].MoveTo(new Vector2f(X + CellWidth * cellX, Y + CellHeight * cellY), true);
                }
            }

            Selected = null;
            SelectedByClick = null;
        }

        public void Swap(int x1, int y1, int x2, int y2, bool reverted = false)
        {
            IsBlocked = true;
            var tween = grid[x1, y1].Swap(grid[x2, y2]);
            OnUpdate += tween.Update;
            tween.OnFinished += () => {
                --TweenCount;
                OnUpdate -= tween.Update;
                Helpers.Swap(ref grid[x1, y1], ref grid[x2, y2]);
                if (reverted) {
                    IsBlocked = false;
                    return;
                }

                FindMatches();
                if (matchCount > 0) {
                    GridChanged();
                } else {
                    Swap(x1, y1, x2, y2, true);
                }
            };
            ++TweenCount;
        }

        private void FindMatches()
        {
            int length = 1;
            matchCount = 0;

            // Horizontal
            for (int y = 0; y < Height; ++y) {
                length = 1;
                for (int x = 0; x < Width; ++x) {
                    matches[x, y] = false;
                    if (x < Width - 1 && grid[x, y].Type == grid[x + 1, y].Type) {
                        ++length;
                    } else {
                        for (int i = 0; length > 2 && i < length; ++i) {
                            matches[x - i, y] = true;
                            ++matchCount;
                        }
                        length = 1;
                    }
                }
            }

            // Vertical
           for (int x = 0; x < Width; ++x) {
                length = 1;
                for (int y = 0; y < Height; ++y) {
                    if (y < Height - 1 && grid[x, y].Type == grid[x, y + 1].Type) {
                        ++length;
                    } else {
                        for (int i = 0; length > 2 && i < length; ++i) {
                            matches[x, y - i] = true;
                            ++matchCount;
                        }
                        length = 1;
                    }
                }
            }
        }

        private int RandomTile()
        {
            return GameManager.Rand.Next(ResourceManager.Tiles.Count);
        }

        public bool IsAdjacent(int x1, int y1, int x2, int y2)
        {
            var dx = Math.Abs(x1 - x2);
            var dy = Math.Abs(y1 - y2);
            return (dx == 1 && dy == 0) || (dy == 1 && dx == 0);
        }

        public bool IsInGrid(int x, int y)
        {
            return x > X && x < X + RealWidth && y > Y && y < Y + RealHeight;
        }

        #endregion
    }
}
