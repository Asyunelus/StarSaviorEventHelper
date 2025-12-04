using EndoAshu.StarSavior.Core;
using System.Windows.Media;
using System.Windows;

namespace StarSaviorAssistant
{
    public static class ThemeManager
    {
        public const string PRIMARY_COLOR_KEY = "PrimaryColor";
        public const string SECONDARY_COLOR_KEY = "SecondaryColor";
        public const string TERTIARY_COLOR_KEY = "TertiaryColor";
        public const string MAIN_BG_KEY = "MainBackground";
        public const string MAIN_FG_KEY = "MainForeground";
        public const string CARD_BG_KEY = "CardBackground";
        public const string MAIN_BORDER_KEY = "MainBorder";
        public const string SUB_TEXT_KEY = "SubText";

        public const string BUTTON_FG_KEY = "ButtonForeground";

        public const string ACCENT_BLUE_COLOR = "AccentColor_Blue_Color";
        public const string ACCENT_PURPLE_COLOR = "AccentColor_Purple_Color";
        public const string ACCENT_ORANGE_COLOR = "AccentColor_Orange_Color";
        public const string ACCENT_GREEN_COLOR = "AccentColor_Green_Color";

        public const string BASE_LIGHT_BG = "Base_Light_Background_Color";
        public const string BASE_LIGHT_FG = "Base_Light_Foreground_Color";
        public const string BASE_LIGHT_CARD = "Base_Light_CardBackground_Color";
        public const string BASE_LIGHT_BORDER = "Base_Light_Border_Color";
        public const string BASE_LIGHT_SUB = "Base_Light_SubText_Color";

        public const string BASE_DARK_BG = "Base_Dark_Background_Color";
        public const string BASE_DARK_FG = "Base_Dark_Foreground_Color";
        public const string BASE_DARK_CARD = "Base_Dark_CardBackground_Color";
        public const string BASE_DARK_BORDER = "Base_Dark_Border_Color";
        public const string BASE_DARK_SUB = "Base_Dark_SubText_Color";

        public static void SetTheme(string themeTag)
        {
            ResourceDictionary appResources = Application.Current.Resources;

            string baseBg, baseFg, baseCard, baseBorder, baseSub;
            string primaryColor, secondaryColor, tertiaryColor;
            string buttonFg;

            primaryColor = ACCENT_BLUE_COLOR;
            secondaryColor = ACCENT_ORANGE_COLOR;
            tertiaryColor = ACCENT_GREEN_COLOR;
            buttonFg = BASE_DARK_FG;

            switch (themeTag)
            {
                case "Light":
                    baseBg = BASE_LIGHT_BG;
                    baseFg = BASE_LIGHT_FG;
                    baseCard = BASE_LIGHT_CARD;
                    baseBorder = BASE_LIGHT_BORDER;
                    baseSub = BASE_LIGHT_SUB;
                    buttonFg = BASE_DARK_FG;
                    break;
                case "Dark":
                    baseBg = BASE_DARK_BG;
                    baseFg = BASE_DARK_FG;
                    baseCard = BASE_DARK_CARD;
                    baseBorder = BASE_DARK_BORDER;
                    baseSub = BASE_DARK_SUB;
                    buttonFg = BASE_DARK_FG;
                    break;
                case "Blue":
                    baseBg = BASE_LIGHT_BG;
                    baseFg = BASE_LIGHT_FG;
                    baseCard = BASE_LIGHT_CARD;
                    baseBorder = BASE_LIGHT_BORDER;
                    baseSub = BASE_LIGHT_SUB;
                    primaryColor = ACCENT_BLUE_COLOR;
                    buttonFg = BASE_DARK_FG;
                    break;
                case "Purple":
                    baseBg = BASE_LIGHT_BG;
                    baseFg = BASE_LIGHT_FG;
                    baseCard = BASE_LIGHT_CARD;
                    baseBorder = BASE_LIGHT_BORDER;
                    baseSub = BASE_LIGHT_SUB;
                    primaryColor = ACCENT_PURPLE_COLOR;
                    buttonFg = BASE_DARK_FG;
                    break;
                default:
                    baseBg = BASE_DARK_BG;
                    baseFg = BASE_DARK_FG;
                    baseCard = BASE_DARK_CARD;
                    baseBorder = BASE_DARK_BORDER;
                    baseSub = BASE_DARK_SUB;
                    buttonFg = BASE_DARK_FG;
                    break;
            }

            UpdateBrush(appResources, PRIMARY_COLOR_KEY, (Color)appResources[primaryColor]);
            UpdateBrush(appResources, SECONDARY_COLOR_KEY, (Color)appResources[secondaryColor]);
            UpdateBrush(appResources, TERTIARY_COLOR_KEY, (Color)appResources[tertiaryColor]);

            UpdateBrush(appResources, MAIN_BG_KEY, (Color)appResources[baseBg]);
            UpdateBrush(appResources, MAIN_FG_KEY, (Color)appResources[baseFg]);
            UpdateBrush(appResources, CARD_BG_KEY, (Color)appResources[baseCard]);
            UpdateBrush(appResources, MAIN_BORDER_KEY, (Color)appResources[baseBorder]);
            UpdateBrush(appResources, SUB_TEXT_KEY, (Color)appResources[baseSub]);

            UpdateBrush(appResources, BUTTON_FG_KEY, (Color)appResources[buttonFg]);

            Settings.Theme = themeTag;
        }

        private static void UpdateBrush(ResourceDictionary resources, string key, Color newColor)
        {
            if (resources[key] is SolidColorBrush existingBrush)
            {
                SolidColorBrush newBrush = existingBrush.Clone();
                newBrush.Color = newColor;
                resources[key] = newBrush;
            }
        }
    }
}