﻿using EOTools.Tools;
using Microsoft.Win32;
using EOTools.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace EOTools.Translation
{
    /// <summary>
    /// Interaction logic for TranslationQuest.xaml
    /// </summary>
    public partial class TranslationShipForm : Page, INotifyPropertyChanged
    {
        private string FilePath
        {
            get
            {
                return AppSettings.ShipTLFilePath;
            }
            set
            {
                AppSettings.ShipTLFilePath = value;
            }
        }

        private JObject JsonShipData = new JObject();

        private GitManager GitManager
        {
            get
            {
                string _gitPath = Path.GetDirectoryName(FilePath);
                return new GitManager(_gitPath);
            }
        }

        private ShipData selectedShip;
        public ShipData SelectedShip
        {
            get
            {
                return selectedShip;
            }
            set
            {
                selectedShip = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ShipData> JsonShip { get; set; } = new ObservableCollection<ShipData>();
        public ObservableCollection<ShipData> JsonSuffixe { get; set; } = new ObservableCollection<ShipData>();
        public ObservableCollection<ShipData> JsonStype { get; set; } = new ObservableCollection<ShipData>();

        private string Version = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public TranslationShipForm()
        {
            this.DataContext = this;
            InitializeComponent();

            if (!string.IsNullOrEmpty(FilePath))
            {
                LoadFile();
            }
        }

        private void LoadFile()
        {
            JsonShipData = JsonHelper.ReadJsonObject(FilePath);

            // --- Ships
            Version = JsonShipData["version"].ToString();

            LoadShipDataFromJObject(JsonShip, (JObject)JsonShipData["ship"]);
            LoadShipDataFromJObject(JsonSuffixe, (JObject)JsonShipData["suffix"]);
            LoadShipDataFromJObject(JsonStype, (JObject)JsonShipData["stype"]);
        }

        private void LoadShipDataFromJObject(ObservableCollection<ShipData> _list, JObject _object)
        {
            _list.Clear();

            foreach (JProperty _shipKey in _object.Properties())
            {
                string _nameEN = _shipKey.Name;
                string _nameJP = _object[_nameEN].ToString();

                ShipData _newShip = new ShipData(_nameEN, _nameJP);

                _list.Add(_newShip);
            }
        }

        private void StageAndPushFiles()
        {
            GitManager.Stage(FilePath);

            string _updatePath = Path.Combine(Path.GetDirectoryName(FilePath), "update.json");
            GitManager.Stage(_updatePath);

            GitManager.CommitAndPush($"Ships - {Version}");
        }

        private Dictionary<string, string> DictionnaryFromShipDataList(IEnumerable<ShipData> _shipList)
        {
            Dictionary<string, string> _dictionary = new Dictionary<string, string>();

            foreach (ShipData _shipData in _shipList)
            {
                _dictionary.Add(_shipData.NameJP, _shipData.NameEN);
            }

            return _dictionary;
        }

        #region Events
        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void buttonSelectFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() != true) return;

            // --- Load file
            FilePath = openFileDialog.FileName;

            try
            {
                LoadFile();
            }
            catch
            {
                MessageBox.Show("Error parsing Json");
            }
        }

        private void ListQuests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;

            SelectedShip = (ShipData)e.AddedItems[0];
        }

        private void buttonAddQuestTL_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ShipData _newShip = new ShipData("", "");

            JsonShip.Add(_newShip);
            SelectedShip = _newShip;
        }

        private void buttonAddSuffix_Click(object sender, RoutedEventArgs e)
        {
            ShipData _newShip = new ShipData("", "");

            JsonSuffixe.Add(_newShip);
            SelectedShip = _newShip;
        }

        private void buttonAddSType_Click(object sender, RoutedEventArgs e)
        {
            ShipData _newShip = new ShipData("", "");

            JsonStype.Add(_newShip);
            SelectedShip = _newShip;
        }

        private void buttonExport_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            Version = (int.Parse(Version) + 1).ToString();

            Dictionary<string, object> _toSerialize = new Dictionary<string, object>();

            _toSerialize.Add("version", Version);

            _toSerialize.Add("ship", DictionnaryFromShipDataList(JsonShip));
            _toSerialize.Add("suffix", DictionnaryFromShipDataList(JsonSuffixe));
            _toSerialize.Add("stype", DictionnaryFromShipDataList(JsonStype));

            JsonHelper.WriteJson(FilePath, _toSerialize);

            // --- Change update.json too
            string _updatePath = Path.Combine(Path.GetDirectoryName(FilePath), "update.json");
            JObject _update = JsonHelper.ReadJsonObject(_updatePath);

            _update["tl_ver"]["ship"] = Version;

            JsonHelper.WriteJson(_updatePath, _update);

            // --- Stage & push
            StageAndPushFiles();
        }

        private void ListShips_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                JsonShip.Remove((ShipData)ListShips.SelectedItem);
            }
        }

        private void ListSuffixe_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                JsonSuffixe.Remove((ShipData)ListSuffixe.SelectedItem);
            }
        }

        private void ListStype_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                JsonStype.Remove((ShipData)ListStype.SelectedItem);
            }
        }
        #endregion
    }
}
