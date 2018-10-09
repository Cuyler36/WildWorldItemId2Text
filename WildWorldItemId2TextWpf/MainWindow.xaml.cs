using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

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

        private readonly List<ushort> _itemIdList;
        private readonly List<string> _itemNameList;

        public MainWindow()
        {
            InitializeComponent();

            // Read item database & load it
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Items)))
            {
                using (var reader = new StreamReader(stream))
                {
                    (_itemIdList, _itemNameList) = LoadWildWorldItemDatabase(reader);
                }
            }

            // Create the list view resource
            var stringList = GetItemListStrings();

            // Set the data context of the list view
            ItemListBox.ItemsSource = stringList;

            // Setup ListView stuff
            var view = CollectionViewSource.GetDefaultView(ItemListBox.ItemsSource);
            view.Filter = o =>
                string.IsNullOrWhiteSpace(SearchTextBox.Text) || o is string str && str.Contains(SearchTextBox.Text);

            SearchTextBox.TextChanged += (_, __) => view.Refresh();
            ItemListBox.SelectionChanged += (_, __) =>
                OutTextBlock.Text = Regex.Replace((string) ItemListBox.SelectedItem, @"\s+", " ").Replace("->", "\n");
        }

        private IEnumerable<string> GetItemListStrings()
        {
            var strings = new List<string>();
            for (var i = 0; i < _itemIdList.Count; i++)
            {
                strings.Add(GetStringForItem(i));
            }

            return strings;
        }

        private string GetStringForItem(int idx)
        {
            var text = BitConverter.GetBytes(_itemIdList[idx])
                .Aggregate("", (current, b) => current + WwJapaneseCharacterDictionary[b]).Replace("\0", "\\0")
                .Replace("\n", "\\n").Replace(" ", "\\s");

            return _itemNameList[idx].PadRight(30) + $" -> {text}";
        }

        private static (List<ushort>, List<string>) LoadWildWorldItemDatabase(StreamReader reader)
        {
            var itemIdList = new List<ushort>();
            var itemNameList = new List<string>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine()?.Trim() ?? "";
                if (line.StartsWith("//")) continue;

                var itemId = line.Substring(0, 6);
                if (!itemId.StartsWith("0x")) continue;

                try
                {
                    itemIdList.Add(ushort.Parse(itemId.Replace("0x", ""), NumberStyles.HexNumber));
                }
                catch
                {
                    Debug.WriteLine($"Error in loading item: {line}");
                    continue;
                }

                // Add item name
                itemNameList.Add(line.Substring(8));
            }

            return (itemIdList, itemNameList);
        }
    }
}
