﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    class Collision
    {
        public Game1 game;

        public bool IsColliding(Sprite hero, Sprite otherSprite)
        {
            // Compares the position of each rectangles edges against the other.
            // it compares opposite edges of each rectangle, ie, the left edge of one with the right of the other
            if (hero.rightEdge < otherSprite.leftEdge || hero.leftEdge > otherSprite.rightEdge || hero.bottomEdge < otherSprite.topEdge || hero.topEdge > otherSprite.bottomEdge)
            {
                // These two rectangles are not colliding
                return false;
            }
            //Otherwise, the two AABB rectangles 
            return true;
        }

        bool CheckForTile(Vector2 coordinates) // Checks if there is a title at the specified coordinates
        {
            int column = (int)coordinates.X;
            int row = (int)coordinates.Y;

            if (column < 0 || column > game.levelTileWidth - 1)
            {
                return false;
            }
            if (row < 0 || row > game.levelTileHeight - 1)
            {
                return true;
            }

            Sprite tileFound = game.levelGrid[column, row];

            if (tileFound != null)
            {
                return true;
            }

            return false;
        }

        Sprite CollideLeft(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            if (IsColliding(playerPrediction, tile) == true && hero.velocity.X < 0)
            {
                hero.position.X = tile.rightEdge + hero.offset.X;
                hero.velocity.X = 0;
            }

            return hero;
        }

        Sprite CollideRight(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            if (IsColliding(playerPrediction, tile) == true && hero.velocity.X > 0)
            {
                hero.position.X = tile.leftEdge - hero.width + hero.offset.X;
                hero.velocity.X = 0;
            }

            return hero;
        }

        Sprite CollideTop(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            if (IsColliding(playerPrediction, tile) == true && hero.velocity.Y < 0)
            {
                hero.position.Y = tile.bottomEdge + hero.offset.Y;
                hero.velocity.Y = 0;
            }

            return hero;
        }

        Sprite CollideBottom(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            if (IsColliding(playerPrediction, tile) == true && hero.velocity.Y > 0)
            {
                hero.position.Y = tile.topEdge - hero.height + hero.offset.Y;
                hero.velocity.Y = 0;
                hero.canJump = true;
            }

            return hero;
        }

        Sprite CollideBottomDiagonals(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];
            int leftEdgeDistance = Math.Abs(tile.leftEdge - playerPrediction.rightEdge);
            int rightEdgeDistance = Math.Abs(tile.rightEdge - playerPrediction.leftEdge);
            int topEdgeDistance = Math.Abs(tile.topEdge - playerPrediction.bottomEdge);

            if (IsColliding(playerPrediction, tile) == true)
            {
                if (topEdgeDistance < rightEdgeDistance && topEdgeDistance < leftEdgeDistance)
                {
                    // If the top edge is closest, the collision is happening to the above of the platform
                    hero.position.Y = tile.topEdge - hero.height + hero.offset.Y;
                    hero.canJump = true;
                    // hero.velocity.Y = 0;
                }
                else if (rightEdgeDistance < leftEdgeDistance)
                {
                    // If the right edge is closest, the collision is happening to the right of the platform
                    hero.position.X = tile.rightEdge + hero.offset.X;
                    // hero.velocity.X = 0;
                }
                else
                {
                    // else if the left edge is closest, the collision is happening to the left of the platform
                     hero.position.X = tile.leftEdge - hero.width + hero.offset.X;
                    // hero.velocity.X = 0;
                }
            }

            return hero;
        }

        Sprite CollideAboveDiagonals(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];
            int leftEdgeDistance = Math.Abs(tile.rightEdge - playerPrediction.leftEdge);
            int rightEdgeDistance = Math.Abs(tile.leftEdge - playerPrediction.rightEdge);
            int bottomEdgeDistance = Math.Abs(tile.bottomEdge - playerPrediction.topEdge);

            if (IsColliding(playerPrediction, tile) == true)
            {
                if (bottomEdgeDistance < leftEdgeDistance && bottomEdgeDistance < rightEdgeDistance)
                {
                    hero.position.Y = tile.bottomEdge + hero.offset.Y;
                    // hero.velocity.Y = 0;
                }
                else if (leftEdgeDistance < rightEdgeDistance)
                {
                    hero.position.X = tile.rightEdge + hero.offset.X;
                    // hero.velocity.X = 0;
                }
                else
                {
                    hero.position.X = tile.leftEdge - hero.width + hero.offset.X;
                    // hero.velocity.X = 0;
                }
            }

            return hero;
        }

        public Sprite CollideWithPlatforms(Sprite hero, float deltaTime)
        {
            //Create a copy of the hero that will move to where the hero will be in the next frame...
            Sprite playerPrediction = new Sprite();
            playerPrediction.position = hero.position;
            playerPrediction.width = hero.width;
            playerPrediction.height = hero.height;
            playerPrediction.offset = hero.offset;
            playerPrediction.UpdateHitBox();

            playerPrediction.position += hero.velocity * deltaTime;

            int playerColumn = (int)Math.Round(playerPrediction.position.X / game.tileHeight);
            int playerRow = (int)Math.Round(playerPrediction.position.Y / game.tileHeight);

            Vector2 playerTile = new Vector2(playerColumn, playerRow);

            Vector2 leftTile = new Vector2(playerTile.X - 1, playerTile.Y);
            Vector2 rightTile = new Vector2(playerTile.X + 1, playerTile.Y);
            Vector2 topTile = new Vector2(playerTile.X, playerTile.Y - 1);
            Vector2 bottomTile = new Vector2(playerTile.X, playerTile.Y + 1);

            Vector2 bottomLeftTile = new Vector2(playerTile.X - 1, playerTile.Y + 1);
            Vector2 bottomRightTile = new Vector2(playerTile.X + 1, playerTile.Y + 1);
            Vector2 topLeftTitle = new Vector2(playerTile.X - 1, playerTile.Y - 1);
            Vector2 topRightTile = new Vector2(playerTile.X + 1, playerTile.Y - 1);
            // ...This allows us to predict if the hero will be overlapping an obstacle in the next frame

            bool leftCheck = CheckForTile(leftTile);
            bool rightCheck = CheckForTile(rightTile);
            bool topCheck = CheckForTile(topTile);
            bool bottomCheck = CheckForTile(bottomTile);

            bool bottomLeftCheck = CheckForTile(bottomLeftTile);
            bool bottomRightCheck = CheckForTile(bottomRightTile);
            bool topLeftCheck = CheckForTile(topLeftTitle);
            bool topRightCheck = CheckForTile(topRightTile);

            if (leftCheck == true) // Check for collisions with the tiles left of the player
            {
                hero = CollideLeft(hero, leftTile, playerPrediction);
            }

            if (rightCheck == true) // Check for collisions with the tiles left of the player
            {
                hero = CollideRight(hero, rightTile, playerPrediction);
            }

            if (topCheck == true) // Check for collisions with the tiles left of the player
            {
                hero = CollideTop(hero, topTile, playerPrediction);
            }

            if (bottomCheck == true) // Check for collisions with the tiles left of the player
            {
                hero = CollideBottom(hero, bottomTile, playerPrediction);
            }

            // Check for collisions with the tiles below and to the left of the player...
            if (leftCheck == false && bottomCheck == false && bottomLeftCheck == true)
            {
                //... then properly check for the diagonals
                hero = CollideBottomDiagonals(hero, bottomLeftTile, playerPrediction);
            }

            // Check for collisions with the tiles below and to the right of the player...
            if (rightCheck == false && bottomCheck == false && bottomRightCheck == true)
            {
                // ... then properly check for the diagonals.
                hero = CollideBottomDiagonals(hero, bottomRightTile, playerPrediction);
            }

            // check for collisions with the tiles above and to the left of the player...
            if (leftCheck == false && topCheck == false && topLeftCheck == true)
            {
                //... then properly check the diagonal
                hero = CollideAboveDiagonals(hero, topLeftTitle, playerPrediction);
            }

            if (rightCheck == false && topCheck == false && topRightCheck == true)
            {
                hero = CollideAboveDiagonals(hero, topRightTile, playerPrediction);
            }

            return hero;
        }

        public Sprite CollideWithMonster(Player hero, Enemy monster, float deltaTime, Game1 theGame)
        {
            Sprite playerPrediction = new Sprite();
            playerPrediction.position = hero.playerSprite.position;
            playerPrediction.width = hero.playerSprite.width;
            playerPrediction.height = hero.playerSprite.height;
            playerPrediction.offset = hero.playerSprite.offset;
            playerPrediction.UpdateHitBox();

            playerPrediction.position += hero.playerSprite.velocity * deltaTime;

            // If there is a collision...
            if (IsColliding(hero.playerSprite, monster.enemySprite))
            {
                int leftEdgeDistance = Math.Abs(monster.enemySprite.leftEdge - playerPrediction.rightEdge);
                int rightEdgeDistance = Math.Abs(monster.enemySprite.rightEdge - playerPrediction.leftEdge);
                int topEdgeDistance = Math.Abs(monster.enemySprite.topEdge - playerPrediction.topEdge);
                int bottomEdgeDistance = Math.Abs(monster.enemySprite.topEdge - playerPrediction.topEdge);

                // ...Check which edge of the monster sprite is closest, then..
                if (topEdgeDistance < leftEdgeDistance && topEdgeDistance < rightEdgeDistance && topEdgeDistance < bottomEdgeDistance)
                {
                    //... then kill the enemy, otherwise...
                    theGame.enemies.Remove(monster);
                    hero.playerSprite.velocity.Y -= hero.jumpStrength * deltaTime;
                    hero.playerSprite.canJump = false;
                }
                else
                {
                    //... the player dies
                    theGame.Exit(); // we will make this work with the health soonish
                }
            }

            return hero.playerSprite;
        }
    }
}
