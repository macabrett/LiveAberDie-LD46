namespace Macabre2D.Project.Gameplay.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ListExtensions {

        public static IEnumerable<T> Shuffle<T>(this IList<T> list) {
            var random = new Random();
            var n = list.Count;
            var shuffledList = list.ToList();
            while (n > 1) {
                n--;
                var k = random.Next(n + 1);
                var value = shuffledList[k];
                shuffledList[k] = shuffledList[n];
                shuffledList[n] = value;
            }

            return shuffledList;
        }
    }
}