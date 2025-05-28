using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class WWWUtils
{
	/// <summary>
	/// Read all the bytes from given URL.
	/// </summary>
	/// <param name="URL">File URL.</param>
    public static async Task<byte[]> GetBytesFromURL (string URL)
    {
        using var www = new WWW (URL);
        while (!www.isDone) {
            Task.Yield();
            if (www.error != null) {
                throw new Exception(www.error);
            }
        }
        if (www.bytes is { Length: > 0 })
            return www.bytes;
        else throw new Exception("read bytes is zero!");
    }
}


