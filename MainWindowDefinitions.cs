using FRESHMusicPlayer.Handlers;
using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI = Gtk.Builder.ObjectAttribute;

namespace FRESHMusicPlayer
{
    partial class MainWindow : Window
    {
        // Import Tab
        [UI] Button BrowseTracksButton = null;
        // Tracks Tab
        [UI] Label Tracks_InfoLabel = null;
        [UI] Button Tracks_DeleteButton = null;
        [UI] Button Tracks_EnqueueButton = null;
        [UI] Button Tracks_PlayButton = null;
        [UI] ListBox Tracks_ContentPanel = null;
        // Controls Box
        [UI] Label TitleLabel = null;
        [UI] Label ArtistLabel = null;
        [UI] Label ProgressLabel1 = null;
        [UI] Label ProgressLabel2 = null;
        [UI] Scale SeekBar = null;
        [UI] Scale VolumeBar = null;
        [UI] Button PreviousButton = null;
        [UI] ToggleButton ShuffleButton = null;
        [UI] Button PlayPauseButton = null;
        [UI] ToggleButton RepeatOneButton = null;
        [UI] Button NextButton = null;
        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);
            progressTimer.Elapsed += Timer_Elapsed;
            player.SongChanged += Player_SongChanged;
            player.SongStopped += Player_SongStopped;
            player.SongException += Player_SongException;
            Title = windowName;
            // Import Tab
            BrowseTracksButton.Clicked += BrowseTracksButton_Clicked;
            // Tracks Tab
            Tracks_DeleteButton.Clicked += Tracks_DeleteButton_Clicked;
            Tracks_EnqueueButton.Clicked += Tracks_EnqueueButton_Clicked;
            Tracks_PlayButton.Clicked += Tracks_PlayButton_Clicked;
            // Controls Box
            NextButton.Clicked += NextButton_Clicked;
            RepeatOneButton.Clicked += RepeatOneButton_Clicked;
            PlayPauseButton.Clicked += PlayPauseButton_Clicked;
            ShuffleButton.Clicked += ShuffleButton_Clicked;
            PreviousButton.Clicked += PreviousButton_Clicked;
            DeleteEvent += Window_DeleteEvent;
            VolumeBar.ValueChanged += VolumeBar_ValueChanged;
            VolumeBar.SetRange(0, 1);
            databaseUtils = new DatabaseUtils(library);
            LoadLibrary();
        }

        public async void LoadLibrary()
        {
            /*
                var tracks = databaseUtils.Read();
                foreach (var track in tracks)
                {
                    var row = new ListBoxRow();
                    var label = new Label();
                    label.Text = $"{track.Artist} - {track.Title}";
                    row.Add(label);
                    Tracks_ContentPanel.Add(row);
                }
            Tracks_ContentPanel.ShowAll();
            */
        }
        private void Tracks_PlayButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Tracks_EnqueueButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Tracks_DeleteButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
