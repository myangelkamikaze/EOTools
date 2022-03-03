﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EOTools.Tools
{
    public static class KancolleTools
    {
        private static List<int> resource = new List<int>() { 6657, 5699, 3371, 8909, 7719, 6229, 5449, 8561, 2987, 5501, 3127, 9319, 4365, 9811, 9927, 2423, 3439, 1865, 5925, 4409, 5509, 1517, 9695, 9255, 5325, 3691, 5519, 6949, 5607, 9539, 4133, 7795, 5465, 2659, 6381, 6875, 4019, 9195, 5645, 2887, 1213, 1815, 8671, 3015, 3147, 2991, 7977, 7045, 1619, 7909, 4451, 6573, 4545, 8251, 5983, 2849, 7249, 7449, 9477, 5963, 2711, 9019, 7375, 2201, 5631, 4893, 7653, 3719, 8819, 5839, 1853, 9843, 9119, 7023, 5681, 2345, 9873, 6349, 9315, 3795, 9737, 4633, 4173, 7549, 7171, 6147, 4723, 5039, 2723, 7815, 6201, 5999, 5339, 4431, 2911, 4435, 3611, 4423, 9517, 3243 };

        private static int GetKey(string _string)
        {
            int _r = 0;

            foreach (char _char in _string)
            {
                _r += _char;
            }

            return _r;
        }

        /// <summary>
        /// Tanaka black magic
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static string GetObfuscatedKey(int _id, string _type)
        {
            int key = GetKey(_type);

            return (17 * (_id + 7) * resource[(key + _id * _type.Length) % 100] % 8973 + 1000).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static string GetFileName (int _id, string _type, string _suffix, string _extension, string _eors, string _filenameWithVersion)
        {
            string key = GetObfuscatedKey(_id, $"{_eors}_{_type}");

            return $"{_id.ToString().PadLeft(4, '0')}{_suffix}_{key}_{_filenameWithVersion}{_extension}";
        }
    }
}
