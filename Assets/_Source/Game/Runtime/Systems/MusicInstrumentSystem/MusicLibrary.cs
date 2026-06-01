using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Application = UnityEngine.Device.Application;
using Debug = UnityEngine.Debug;

namespace Game.Runtime.MusicInstrumentSystem
{
    public class MusicLibrary
    {
        [Serializable]
        private class MusicInfo
        {
            public string Name;
            public int Difficulty;
        }
        
        private readonly IEnumerable<IMusicFileReader> _musicFileReaders;
        private readonly List<MusicTrack> _musicTracks = new();

        public IEnumerable<MusicTrack> MusicTracks => _musicTracks;
        
        public MusicLibrary(IEnumerable<IMusicFileReader> musicFileReaders)
        {
            _musicFileReaders = musicFileReaders;
        }
        
        public void Initialize()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "MusicLibrary"));
            DirectoryInfo[] folders = dirInfo.GetDirectories();

            Dictionary<MusicalInstrumentType, Note[]> notes = new();
            int musicTrackId = 0;
            for (int folderIndex = 0; folderIndex < folders.Length; folderIndex++)
            {
                notes.Clear();
                
                FileInfo[] tracks = folders[folderIndex].GetFiles();
                MusicInfo info = null;
                for (int fileIndex = 0; fileIndex < tracks.Length; fileIndex++)
                {
                    string filePath = tracks[fileIndex].ToString();
                    if (tracks[fileIndex].Extension == ".json")
                    {
                        try
                        {
                            if(info == null)
                                info = JsonUtility.FromJson<MusicInfo>(File.ReadAllText(filePath));
                        }
                        catch
                        {
                            continue;
                        }

                        continue;
                    }

                    MusicalInstrumentType musicalInstrumentType;
                    try
                    {
                        musicalInstrumentType = Enum.Parse<MusicalInstrumentType>(Path.GetFileNameWithoutExtension(tracks[fileIndex].Name), false);
                    }
                    catch
                    {
                        continue;
                    }
                    foreach (var fileReader in _musicFileReaders)
                    {
                        if (!fileReader.FileIsValid(filePath)) continue;
                        notes.Add(musicalInstrumentType, fileReader.GetNotes(filePath).ToArray());
                        break;
                    }
                }
                if (notes.Count == 0 || info == null) return;
                var musicTrack = new MusicTrack(musicTrackId, info.Name, info.Difficulty);
                foreach (var typeNotesPair in notes)
                {
                    musicTrack.Notes.Add(typeNotesPair.Key, typeNotesPair.Value);
                }
                _musicTracks.Add(musicTrack);
                musicTrackId++;
            }
        }
        
        public MusicTrack GetMusicTrack(int id)
        {
            return _musicTracks[id];
        }
    }
}