using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
   public static T[] FromJson<T>(string json)
   {
      Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
      return wrapper.Levels;
   }

   public static string ToJson<T>(T[] array)
   {
      Wrapper<T> wrapper = new Wrapper<T>();
      wrapper.Levels = array;
      return JsonUtility.ToJson(wrapper);
   }

   public static string ToJson<T>(List<T> array, bool prettyPrint, List<T> dataInJson)
   {
      Wrapper<T> wrapper = new Wrapper<T>();

      if (dataInJson.Count != 0)
      {
         dataInJson.AddRange(array);
         wrapper.Levels = dataInJson.ToArray();
      }
      else
      {
         wrapper.Levels = array.ToArray();
      }

      return JsonUtility.ToJson(wrapper, prettyPrint);
   }
}
