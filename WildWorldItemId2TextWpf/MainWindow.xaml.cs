using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WildWorldItemId2TextWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static readonly string[] WwJapaneseCharacterDictionary = {
            "\0", "あ", "い", "う", "え", "お", "か", "き", "く", "け", "こ", "さ", "し", "す", "せ", "そ",
            "た", "ち", "つ", "て", "と", "な", "に", "ぬ", "ね", "の", "は", "ひ", "ふ", "へ", "ほ", "ま",
            "み", "む", "め", "も", "や", "ゆ", "よ", "ら", "り", "る", "れ", "ろ", "わ", "を", "ん", "が",
            "ぎ", "ぐ", "げ", "ご", "ざ", "じ", "ず", "ぜ", "ぞ", "だ", "ぢ", "づ", "で", "ど", "ば", "び",
            "ぶ", "べ", "ぼ", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ", "ぁ", "ぃ", "ぅ", "ぇ", "ぉ", "ゃ", "ゅ", "ょ",
            "っ", "ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ", "サ", "シ", "ス", "セ", "ソ",
            "タ", "チ", "ツ", "テ", "ト", "ナ", "二", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "へ", "ホ", "マ",
            "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ", "ル", "レ", "ロ", "ワ", "ヲ", "ソ", "ガ",
            "ギ", "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ", "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ",
            "ブ", "ベ", "ボ", "パ", "ピ", "プ", "ペ", "ポ", "ァ", "ィ", "ゥ", "ェ", "ォ", "ャ", "ュ", "ョ",
            "ッ", "ヴ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N",
            "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d",
            "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
            "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            " ", "\n", "ー", "~", "･", "。", "、", "!", "?", ".", ",", "｢", "｣", "(", ")", "<",
            ">", "'", "\"", "_", "+", "=", "&", "@", ":", ";", "×", "÷", "🌢", "★", "♥", "♪"
        };

        public MainWindow()
        {
            InitializeComponent();

            ConvertButton.Click += (_, __) => ParseItemIdString(ItemIdTextBox.Text);
        }

        private static bool OnlyHexInString(string test) =>
            System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z");

        public void ParseItemIdString(string itemIdStr)
        {
            if (!OnlyHexInString(itemIdStr))
            {
                MessageBox.Show("The item id you entered doesn't appear to be in hexadecimal! Please try again.",
                    "Item Id Parse Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                OutTextBlock.Text = "Invalid";
            }
            else
            {
                var itemId = itemIdStr.Replace("0x", "");
                if ((itemId.Length & 1) != 0)
                {
                    itemId += "0";
                    MessageBox.Show(
                        $"The item id you entered didn't appear to be formatted correctly! It was adjusted to {itemId}!",
                        "Item Id Parse Error",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (ushort.TryParse(itemId, NumberStyles.HexNumber, null, out var hexItemId))
                {
                    var str = BitConverter.GetBytes(hexItemId)
                        .Aggregate("", (current, b) => current + WwJapaneseCharacterDictionary[b]);

                    OutTextBlock.Text = $"Enter the following for to get item id {hexItemId:X4}:\n{str}";
                }
                else
                {
                    MessageBox.Show($"Unable to parse the item id {itemId}! Please try again!", "Item Id Parse Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    OutTextBlock.Text = "Invalid";
                }
            }
        }
    }
}
