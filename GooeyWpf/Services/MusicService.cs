﻿using NAudio.Wave;
using System.IO;

namespace GooeyWpf.Services
{
    internal class MusicService : Singleton<MusicService>
    {
        private const string CacheName = "MusicCache.dat";

        private static readonly string MusicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        private static string MusicPath = Path.Combine(MusicFolder);

        public MusicService()
        {
            if (!File.Exists(CacheName))
            {
                BuildCache();
            }
            else
            {
                ReadCache();
            }
        }

        public List<Music> Library { get; } = [];

        private static void BuildCache()
        {
            List<string> files = [];
            files.AddRange(Directory.GetFiles(MusicPath, "*.mp3", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(MusicPath, "*.flac", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(MusicPath, "*.wav", SearchOption.AllDirectories));

            if (File.Exists(CacheName)) File.Delete(CacheName);

            using FileStream cacheFile = File.Create(CacheName);
            using BinaryWriter cacheWriter = new(cacheFile);
            cacheWriter.Write(files.Count);
            foreach (var file in files)
            {
                TagLib.File taglibFile = TagLib.File.Create(file);

                cacheWriter.Write(file);
                cacheWriter.Write(taglibFile.Tag.Title ?? Path.GetFileNameWithoutExtension(file));
                cacheWriter.Write(taglibFile.Tag.FirstPerformer ?? taglibFile.Tag.FirstAlbumArtist ?? "");
                cacheWriter.Write(taglibFile.Tag.Album ?? "");
            }
        }

        private void ReadCache()
        {
            Library.Clear();

            using FileStream cacheFile = File.OpenRead(CacheName);

            if (cacheFile.Length < sizeof(int))
                BuildCache();

            using BinaryReader cacheReader = new(cacheFile);

            int count = cacheReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Library.Add(new Music(
                    cacheReader.ReadString(),
                    cacheReader.ReadString(),
                    cacheReader.ReadString(),
                    cacheReader.ReadString()));
            }
        }

        public void UpdateLibrary()
        {
            BuildCache();
            ReadCache();
        }

        public void PlayRandom()
        {
            Play(Library[RandomService.Instance.Next(0, Library.Count)]);
        }

        public void Play(Music music)
        {
            MediaFoundationReader mediaFoundationReader = new(music.Path);
            AudioService.Instance.Initialize(mediaFoundationReader);
            AudioService.Instance.Play();
        }

        public void Play(Stream waveStream)
        {
            AudioService.Instance.Initialize(new WaveFileReader(waveStream));
            AudioService.Instance.Play();
        }

        public void PlayMp3(Stream mp3WaveStream)
        {
            AudioService.Instance.Initialize(new Mp3FileReader(mp3WaveStream));
            AudioService.Instance.Play();
        }

        public void Stop()
        {
            AudioService.Instance.Stop();
        }

        public struct Music(string path, string title, string artist, string album)
        {
            public string Path = path;
            public string Title = title;
            public string Artist = artist;
            public string Album = album;
        }
    }
}