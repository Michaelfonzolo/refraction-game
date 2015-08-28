using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DemeterEngine;

namespace Refraction_V2
{
    public static class Assets
    {

        public static class Shared
        {
            public static class Images
            {
                public static readonly string AssetName_LaserCap = Path.Combine("Shared", "Images", "LaserCap");

                public static readonly string AssetName_LaserSegment = Path.Combine("Shared", "Images", "LaserSegment");

                public static readonly Texture2D LaserCap = ArtManager.Texture2D(AssetName_LaserCap);

                public static readonly Texture2D LaserSegment = ArtManager.Texture2D(AssetName_LaserSegment);
            }

            public static class Fonts
            {
                public static readonly string AssetName_GUIButtonFont_Small = Path.Combine("Shared", "Fonts", "GUIButtonFont_Small");

                public static readonly string AssetName_GUIButtonFont_Medium = Path.Combine("Shared", "Fonts", "GUIButtonFont_Medium");

                public static readonly string AssetName_GUIButtonFont_Large = Path.Combine("Shared", "Fonts", "GUIButtonFont_Large");

                public static readonly string AssetName_Error = Path.Combine("Shared", "Fonts", "Error");

                public static readonly SpriteFont GUIButtonFont_Small = ArtManager.Font(AssetName_GUIButtonFont_Small);

                public static readonly SpriteFont GUIButtonFont_Medium = ArtManager.Font(AssetName_GUIButtonFont_Medium);

                public static readonly SpriteFont GUIButtonFont_Large = ArtManager.Font(AssetName_GUIButtonFont_Large);

                public static readonly SpriteFont Error = ArtManager.Font(AssetName_Error);
            }
        }

        public static class Level
        {
            public static class Images
            {
                public static readonly string AssetName_BackButton = Path.Combine("Level", "Images", "BackButton");

                public static readonly string AssetName_InventoryButton = Path.Combine("Level", "Images", "InventoryButton");

                public static readonly string AssetName_InventoryButtonHover = Path.Combine("Level", "Images", "InventoryButtonHover");

                public static readonly string AssetName_EmptyTile = Path.Combine("Level", "Images", "EmptyTile");

                public static readonly string AssetName_Outputter = Path.Combine("Level", "Images", "Outputter");

                public static readonly string AssetName_Receiver = Path.Combine("Level", "Images", "Receiver");

                public static readonly string AssetName_Refractor_DLxDR_D = Path.Combine("Level", "Images", "Refractor_DLxDR_D");

                public static readonly string AssetName_Refractor_DxL_DL = Path.Combine("Level", "Images", "Refractor_DxL_DL");

                public static readonly string AssetName_Refractor_DxR_DR = Path.Combine("Level", "Images", "Refractor_DxR_DR");

                public static readonly string AssetName_Refractor_ULxDL_L = Path.Combine("Level", "Images", "Refractor_ULxDL_L");

                public static readonly string AssetName_Refractor_ULxUR_U = Path.Combine("Level", "Images", "Refractor_ULxUR_U");

                public static readonly string AssetName_Refractor_URxDR_R = Path.Combine("Level", "Images", "Refractor_URxDR_R");

                public static readonly string AssetName_Refractor_UxL_UL = Path.Combine("Level", "Images", "Refractor_UxL_UL");

                public static readonly string AssetName_Refractor_UxR_UR = Path.Combine("Level", "Images", "Refractor_UxR_UR");

                public static readonly string AssetName_TileHover = Path.Combine("Level", "Images", "TileHover");

                public static readonly Texture2D BackButton = ArtManager.Texture2D(AssetName_BackButton);

                public static readonly Texture2D InventoryButton = ArtManager.Texture2D(AssetName_InventoryButton);

                public static readonly Texture2D InventoryButtonHover = ArtManager.Texture2D(AssetName_InventoryButtonHover);

                public static readonly Texture2D EmptyTile = ArtManager.Texture2D(AssetName_EmptyTile);

                public static readonly Texture2D Outputter = ArtManager.Texture2D(AssetName_Outputter);

                public static readonly Texture2D Receiver = ArtManager.Texture2D(AssetName_Receiver);

                public static readonly Texture2D Refractor_DLxDR_D = ArtManager.Texture2D(AssetName_Refractor_DLxDR_D);

                public static readonly Texture2D Refractor_DxL_DL = ArtManager.Texture2D(AssetName_Refractor_DxL_DL);

                public static readonly Texture2D Refractor_DxR_DR = ArtManager.Texture2D(AssetName_Refractor_DxR_DR);

                public static readonly Texture2D Refractor_ULxDL_L = ArtManager.Texture2D(AssetName_Refractor_ULxDL_L);

                public static readonly Texture2D Refractor_ULxUR_U = ArtManager.Texture2D(AssetName_Refractor_ULxUR_U);

                public static readonly Texture2D Refractor_URxDR_R = ArtManager.Texture2D(AssetName_Refractor_URxDR_R);

                public static readonly Texture2D Refractor_UxL_UL = ArtManager.Texture2D(AssetName_Refractor_UxL_UL);

                public static readonly Texture2D Refractor_UxR_UR = ArtManager.Texture2D(AssetName_Refractor_UxR_UR);

                public static readonly Texture2D TileHover = ArtManager.Texture2D(AssetName_TileHover);
            }

            public static class Fonts
            {
                public static readonly string AssetName_InventoryItem = Path.Combine("Level", "Fonts", "InventoryItem");

                public static readonly string AssetName_PauseText = Path.Combine("Level", "Fonts", "PauseText");
            
                public static readonly SpriteFont InventoryItem = ArtManager.Font(AssetName_InventoryItem);

                public static readonly SpriteFont PauseText = ArtManager.Font(AssetName_PauseText);
            }            
        }

        public static class LevelComplete
        {
            public static class Images
            {
                public static readonly string AssetName_BackButton = Path.Combine("LevelComplete", "Images", "BackButton");

                public static readonly string AssetName_PrevButton = Path.Combine("LevelComplete", "Images", "PrevButton");

                public static readonly string AssetName_ReplayButton = Path.Combine("LevelComplete", "Images", "ReplayButton");

                public static readonly string AssetName_NextButton = Path.Combine("LevelComplete", "Images", "NextButton");

                public static readonly Texture2D BackButton = ArtManager.Texture2D(AssetName_BackButton);

                public static readonly Texture2D PrevButton = ArtManager.Texture2D(AssetName_PrevButton);

                public static readonly Texture2D ReplayButton = ArtManager.Texture2D(AssetName_ReplayButton);

                public static readonly Texture2D NextButton = ArtManager.Texture2D(AssetName_NextButton);
            }

            public static class Fonts
            {
                public static readonly string AssetName_LevelCompleteText = Path.Combine("LevelComplete", "Fonts", "LevelCompleteText");

                public static readonly SpriteFont LevelCompleteText = ArtManager.Font(AssetName_LevelCompleteText);
            }
        }

        public static class LevelSelect
        {
            public static class Images
            {
                public static readonly string AssetName_BackButton = Path.Combine("LevelSelect", "Images", "BackButton");

                public static readonly string AssetName_ClearedLevelButton = Path.Combine("LevelSelect", "Images", "ClearedLevelButton");

                public static readonly string AssetName_UnclearedLevelButton = Path.Combine("LevelSelect", "Images", "UnclearedLevelButton");

                public static readonly Texture2D BackButton = ArtManager.Texture2D(AssetName_BackButton);

                public static readonly Texture2D ClearedLevelButton = ArtManager.Texture2D(AssetName_ClearedLevelButton);

                public static readonly Texture2D UnclearedLevelButton = ArtManager.Texture2D(AssetName_UnclearedLevelButton);
            }

            public static class Fonts
            {
                public static readonly string AssetName_LevelNo = Path.Combine("LevelSelect", "Fonts", "LevelNo");

                public static readonly SpriteFont LevelNo = ArtManager.Font(AssetName_LevelNo);
            }
        }

        public static class MainMenu
        {
            public static class Images
            {
                public static readonly string AssetName_PlayButton = Path.Combine("MainMenu", "Images", "PlayButton");

                public static readonly Texture2D PlayButton = ArtManager.Texture2D(AssetName_PlayButton);
            }

            public static class Fonts
            {

            }
        }
    }
}
