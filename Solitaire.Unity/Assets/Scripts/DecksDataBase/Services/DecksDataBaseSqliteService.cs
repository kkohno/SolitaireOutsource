using System;
using System.IO;
using System.Linq;
using SQLite;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Scripts.DecksDataBase.Services
{
    static class GameTypeToDataBaseExtension
    {
        public static byte ToDataBase(this GameType gameType)
        {
            switch (gameType) {
                case GameType.OneOnOne: return 1;
                case GameType.ThreeOnThree:
                    return 3;
                case GameType.OneOnThree:
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
        }
    }
    public sealed class DecksDataBaseSqliteService : IDecksDataBaseService
    {
        const string DATA_BASE_SOURCE_PATH = "data_base.db";
        static string DATA_BASE_PATH = $"{Application.persistentDataPath}/data_base_v15.db";

      /*  public async Task<Layout> GetDeck(GameType type)
        {
            await TryCopyDataBaseSource();
            using var db = new SQLiteConnection(DATA_BASE_PATH);
            var select = db.Table<Layout>().Select(i => i);
            var count = select.Count();
            var layout = select.Skip(Random.Range(0, count)).First();
            return layout;
        }
        public async Task<Solution> GetSolution(int layoutId)
        {
            await TryCopyDataBaseSource();
            using var db = new SQLiteConnection(DATA_BASE_PATH);
            try {
                return db.Table<Solution>().Select(i => i).First(i => i.LayoutId == layoutId);
            }
            catch (Exception ex) {
                Debug.LogError(ex);
                return null;
            }
        }*/

        static async Task TryCopyDataBaseSource()
        {
            var data = await LoadStreamingAssetFile(DATA_BASE_SOURCE_PATH);

            if (File.Exists(DATA_BASE_PATH)) {
                if (data.Length == (await File.ReadAllBytesAsync(DATA_BASE_PATH)).Length)
                    return;
            }
            await using var stream = File.Create(DATA_BASE_PATH);
            await using var writer = new BinaryWriter(stream);
            writer.Write(data);
            Debug.Log($"solitaire file copied to {DATA_BASE_PATH} size={data.Length}");
        }
        static async Task<byte[]> LoadStreamingAssetFile(string fileName)
        {
            //var filePath = "DataBase/" + path.Replace(".json", "");
            //var targetFile = Resources.Load<TextAsset>(filePath);
            //return targetFile.text;

            var path = $"{Application.streamingAssetsPath}/{fileName}";
            Debug.Log($"solitaire:  ReadDataBase {fileName}");
            //Debug.Log($"solitaire:  path1 {path}");
#if UNITY_ANDROID
            //path = $"jar:file://{Application.dataPath}!/assets/{fileName}";
            //var r = new UnityWebRequest(path);
            var r = UnityWebRequest.Get(path);
            await r.SendWebRequest();

            return r.downloadHandler.data;
#else
            return File.ReadAllText(path);
#endif
        }
    }
}