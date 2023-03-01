﻿using PlateauCityGml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CityGMLTest
{
    class Program
    {
        static CityGMLParser parser;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Project PLATEAU の CityGMLファイル(.gml)を .obj 形式に変換します。");
                Console.WriteLine("https://github.com/ksasao/PlateauCityGmlSharp");
                Console.WriteLine();
                Console.WriteLine("基準点を指定しない場合は、モデル全体の緯度、経度（高度＝0）の最小値が原点になります。");
                Console.WriteLine(".obj ファイルは outputフォルダ以下に出力されます。");
                Console.WriteLine();
                Console.WriteLine("使い方: CityGMLToObj .gmlファイルのパス [[基準点となる緯度] [経度] [高度] [緯度の下限] [経度の下限] [緯度の上限] [経度の上限]]");
                Console.WriteLine();
                Console.WriteLine("例1) シンプルな利用方法(テクスチャがある場合はフルパスを指定してください)");
                Console.WriteLine("CityGMLToObj sample.gml");
                Console.WriteLine();
                Console.WriteLine("例2) 北緯 35.000度 東経 135.000度 高度100m を原点として座標を変換");
                Console.WriteLine("CityGMLToObj sample.gml 35.000 135.000 100");
                Console.WriteLine();
                Console.WriteLine("何かキーを押してください...");
                Console.ReadKey();
            }
            else
            {
                try
                {
                    Assembly exePath = Assembly.GetEntryAssembly();
                    string path = Path.Combine(Path.GetDirectoryName(exePath.Location), "output", Path.GetFileNameWithoutExtension(args[0]));
                    if(args.Length == 4)
                    {
                        Position p = new Position
                        {
                            Latitude = Convert.ToDouble(args[1]),
                            Longitude = Convert.ToDouble(args[2]),
                            Altitude = Convert.ToDouble(args[3])
                        };
                        CreateModel(args[0], path, p);
                    }
                    else if(args.Length == 8)
                    {
                        Position p = new Position
                        {
                            Latitude = Convert.ToDouble(args[1]),
                            Longitude = Convert.ToDouble(args[2]),
                            Altitude = Convert.ToDouble(args[3])
                        };
                        Position lower = new Position
                        {
                            Latitude = Convert.ToDouble(args[4]),
                            Longitude = Convert.ToDouble(args[5]),
                        };
                        Position upper = new Position
                        {
                            Latitude = Convert.ToDouble(args[6]),
                            Longitude = Convert.ToDouble(args[7]),
                        };
                        CreateModel(args[0], path, p,lower, upper);

                    }
                    else
                    {
                        CreateModel(args[0], path);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
            }
        }

        static Position defaultPosition = new Position { Latitude = 1000 };
        static void CreateModel(string path, string outputPath)
        {
            CreateModel(path, outputPath, defaultPosition);
        }
        static void CreateModel(string path,string outputPath, Position origin)
        {
            var parser = new CityGMLParser();
            origin = ParseFile(path, outputPath, origin, parser);

        }
        static void CreateModel(string path, string outputPath, Position origin, Position lower, Position upper)
        {
            var parser = new CityGMLParser();
            parser.LowerCorner = lower;
            parser.UpperCorner = upper;

            origin = ParseFile(path, outputPath, origin, parser);

        }

        private static Position ParseFile(string path, string outputPath, Position origin, CityGMLParser parser)
        {
            var coType = parser.GetCityObjectType(path);
            if (coType == CityObjectType.Building)
            {
                origin = CreateBuilding(path, outputPath, origin, parser);
            }
            else if (coType == CityObjectType.Relief)
            {
                origin = CreateRelief(path, outputPath, origin, parser);
            }
            else
            {
                Console.WriteLine("サポートしていない形式です。");
            }

            return origin;
        }

        private static Position CreateBuilding(string path, string outputPath, Position origin, CityGMLParser parser)
        {
            var buildings = parser.GetBuildings(path);

            if (origin == defaultPosition)
            {
                origin = new Position
                {
                    Latitude = buildings.Min(c => c.LowerCorner.Latitude),
                    Longitude = buildings.Min(c => c.LowerCorner.Longitude),
                    //Altitude = buildings.Min(c => c.LowerCorner.Altitude)
                    Altitude = 0
                };
            }
            // ヘッダ
            Console.WriteLine($"Origin: {origin.Latitude},{origin.Longitude},{origin.Altitude}\t\t\t\t\t");
            Console.WriteLine($"ID\tLatitude\tLongitude\tAltitude\tTriangles\tName");
            for (int i = 0; i < buildings.Length; i++)
            {
                try
                {
                    Building b = buildings[i];
                    ModelGenerator mg = new ModelGenerator(b, origin);
                    mg.SaveAsObj(Path.Combine(outputPath, b.Id + ".obj"));

                    // ステータス表示
                    int triangles = b.LOD2Solid != null ? b.LOD2Solid.Length : b.LOD1Solid.Length;
                    Console.WriteLine($"{b.Id}\t{b.LowerCorner.Latitude:F8}\t{b.LowerCorner.Longitude:F8}\t{b.LowerCorner.Altitude}\t{triangles}\t{b.Name}");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return origin;
        }

        private static Position CreateRelief(string path, string outputPath, Position origin, CityGMLParser parser)
        {
            var relief = parser.GetRelief(path);

            if (origin == defaultPosition)
            {
                origin = new Position
                {
                    Latitude = relief.LowerCorner.Latitude,
                    Longitude = relief.LowerCorner.Longitude,
                    Altitude = relief.LowerCorner.Altitude
                };
            }
            // ヘッダ
            Console.WriteLine($"Origin: {origin.Latitude},{origin.Longitude},{origin.Altitude}\t\t\t\t\t");
            Console.WriteLine($"ID\tLatitude\tLongitude\tAltitude\tTriangles\tName");
            Building b = new Building();
            b.LOD1Solid = relief.LOD1Solid;
            b.LowerCorner = relief.LowerCorner;
            b.UpperCorner = relief.UpperCorner;
            b.Id = Path.GetFileNameWithoutExtension(path);
            try
            {
                ModelGenerator mg = new ModelGenerator(b, origin);
                mg.SaveAsObj(Path.Combine(outputPath, b.Id + ".obj"));

                // ステータス表示
                int triangles = b.LOD1Solid.Length;
                Console.WriteLine($"{b.Id}\t{b.LowerCorner.Latitude:F8}\t{b.LowerCorner.Longitude:F8}\t{b.LowerCorner.Altitude}\t{triangles}\t{b.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return origin;
        }
    }
}
