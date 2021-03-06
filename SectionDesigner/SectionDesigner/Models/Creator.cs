﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using Majstersztyk.TS_materials;

namespace Majstersztyk
{
    static class Creator
    {
    	private static string v = "\\$Voids";
    	private static string m = "\\$Material";
    	private static string c = "\\$Contour.txt";
    	private static string b = "\\$Bars.txt";
    	private static string p = "\\$Parts";
    	private static string r = "\\$Reinforcement";
    	private static string pathSec = @"section";
        
    	public static TS_section ReadSection(){
			return new TS_section(ReadParts(pathSec), ReadReoGroups(pathSec));
    	}
    	
    	public static TS_section CreateInitiateSection(){
			List<TS_point> PointsC0 = new List<TS_point>{ new TS_point() };
			TS_contour Contour0 = new TS_contour(PointsC0);
			
			List<TS_point> PointsV0 = new List<TS_point>{ new TS_point() };
			TS_void Void0 = new TS_void(PointsV0);
			Void0.Name = "new Void";
			List<TS_void> Voids0 = new List<TS_void>() { Void0 };
			
			TS_part Part0 = new TS_part(new TS_mat_universal(), Contour0, Voids0);
			Part0.Name = "Part1";
			List<TS_part> Parts0 = new List<TS_part>(){ Part0 };
			
			TS_bar Bar0 = new TS_bar(new TS_point(), Double.NaN);
			List<TS_bar> Bars0 = new List<TS_bar>() { Bar0 };
			TS_reinforcement Reo0 = new TS_reinforcement(Bars0, new TS_mat_universal());
			List<TS_reinforcement> Reos0 = new List<TS_reinforcement>(){ Reo0 };
			
			TS_section Section0 = new TS_section(Parts0, Reos0);
			return Section0;
    	}
    	
        private static List<TS_point> ReadPoints(string pathToTheFile){
            List<TS_point> lista = new List<TS_point>();
			string line;
            using (System.IO.StreamReader file = new System.IO.StreamReader(pathToTheFile)) //"..\..\input.txt")) 
            {
            	while ((line = file.ReadLine()) != null) {
                    string[] coo = line.Split(new char[] { ';' });
                    double x, y;
                    Double.TryParse(coo[0], out x);
                    Double.TryParse(coo[1], out y);
                    lista.Add(new TS_point(x, y));
                }
            }
			return lista;
        }
        
        private static List<TS_bar> ReadBars(string pathToTheFile){
        	List<TS_bar> lista = new List<TS_bar>();
            string line;
            using (System.IO.StreamReader file = new System.IO.StreamReader(@pathToTheFile)) //"..\..\input.txt")) 
            {
                while ((line = file.ReadLine()) != null) {
                    string[] coo = line.Split(new char[] { ';' });
                    double x, y, dia;
                    Double.TryParse(coo[0], out x);
                    Double.TryParse(coo[1], out y);
                    Double.TryParse(coo[2], out dia);
                    lista.Add(new TS_bar(new TS_point(x, y), dia));
                }
            }
            return lista;
        }
        
        private static List<TS_void> ReadVoids(string pathToThePart){
			List<TS_void> result = new List<TS_void>();
			string path = pathToThePart + v;
			List<string> voidsPaths = Directory.GetFiles(path).ToList();
			
			foreach (var voidpath in voidsPaths) {
				string name = FileName(voidpath);
				TS_void thisVoid = new TS_void(ReadPoints(voidpath));
				thisVoid.Name = name;
				result.Add(thisVoid);
			}
			return result;
        }
        
        private static List<TS_reinforcement> ReadReoGroups(string pathToTheSection){
        	List<TS_reinforcement> ReoGr = new List<TS_reinforcement>();
			List<string> rgsPaths = Directory.GetDirectories(pathToTheSection + r).ToList();
			foreach (var path in rgsPaths) {
				List<TS_bar> Bars = ReadBars(path + b);
				TS_materials.TS_material mat = ReadMaterial(path);
				TS_reinforcement Reo = new TS_reinforcement(Bars, mat);
				Reo.Name = FolderName(path);
				ReoGr.Add(Reo);
			}
			return ReoGr;
        }
        
        private static List<TS_part> ReadParts(string pathToTheSection){
			List<TS_part> Parts = new List<TS_part>();
			List<string> partsPaths = Directory.GetDirectories(pathToTheSection + p).ToList();
			foreach (var path in partsPaths) {
				TS_contour cont = ReadContour(path);
				TS_materials.TS_material mat = ReadMaterial(path);
				List<TS_void> voids = ReadVoids(path);
				TS_part Part = new TS_part(mat, cont, voids);
				Part.Name = FolderName(path);
				Parts.Add(Part);
			}
			return Parts;
        }
        
        private static TS_contour ReadContour (string pathToThePart){
			string path = pathToThePart + c;
			TS_contour result = new TS_contour(ReadPoints(path));
            string name = FileName(path);
            result.Name = name;
            return result;
        }
        
        private static TS_materials.TS_mat_universal ReadMaterial(string pathToThePart){
			string path = pathToThePart + m;
			path = Directory.GetFiles(path)[0];
			double E;
			using (System.IO.StreamReader file = new System.IO.StreamReader(@path)) { //"..\..\input.txt")) 
				file.ReadLine();
				string line = file.ReadLine();
				Double.TryParse(line, out E);
			}
			
			return new TS_materials.TS_mat_universal(E, FileName(path));
        }
        
        private static string FileName(string pathToTheFile){
			string[] pathes = pathToTheFile.Split(new char[] { '.', '\\' });
			return pathes[pathes.Length-2];
        }
        
        private static string FolderName(string pathToTheFolder){
			string[] pathes = pathToTheFolder.Split(new char[] {'\\' });
			return pathes[pathes.Length-1];
        }

    	
    	public static void SaveToFile(string path, string content){
			File.WriteAllText(path, content);
    	}
    }
}
