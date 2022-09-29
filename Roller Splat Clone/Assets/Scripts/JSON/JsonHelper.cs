using System.Collections.Generic;
using UnityEngine;

namespace Json
{
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

      public static string ToJson<T>(T element, bool prettyPrint, List<T> dataInJson)
      {
         Wrapper<T> wrapper = new Wrapper<T>();
         
         dataInJson.Add(element);
         wrapper.Levels = dataInJson.ToArray();

         return JsonUtility.ToJson(wrapper, prettyPrint);
      }
   }
}