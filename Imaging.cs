using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Linq;

namespace dcitysim
{
    partial class Program
    {
        public class Imaging
        {
            //variables

            public List<string> colors;
            public List<string> image;
            string data;
            Bitmap b1;
            ProgressBar bar;

            //My classes
            public Imaging()
            {
                image = new List<string>();
                colors = new List<string>();
                bar = new ProgressBar(0, new ProgressBar.ProgressBarStyle('[', ']', '=', '>', '-'), 60);
            }
            //Helper enums
            public enum ActionResult
            {
                Failed = 0,
                Success = 1
            }



            //Image functions
            public ConsoleColor ClosestConsoleColor(byte r, byte g, byte b) //Gets the closest color to the inserted RGB Values         
            {
                ConsoleColor ret = 0;
                double rr = r, gg = g, bb = b, delta = double.MaxValue;

                foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
                {
                    var n = Enum.GetName(typeof(ConsoleColor), cc);
                    var c = System.Drawing.Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
                    var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                    if (t == 0.0)
                        return cc;
                    if (t < delta)
                    {
                        delta = t;
                        ret = cc;
                    }
                }
                return ret;
            }

            public void Resize(string path,int w) //Resizes image with proportions to width 'W'
            {
                if(File.Exists(path+"_s"))
                    File.Delete(path + "_s");
                if (!File.Exists(path))
                    return;
                b1 = new Bitmap(path);
                int scaleF = b1.Width / w;
                Bitmap resized = new Bitmap(b1, new Size(b1.Width / scaleF, b1.Height / scaleF));
                resized.Save(path+"_s");
                Thread.Sleep(10);
                resized.Dispose();
                b1.Dispose();
                Debug("Resized Succesfully. Remember to use " + path + "_s instead of " + path + " while loading");               
            }
            public void FResize(string path, int w,int h) //Resizes image with proportions to width 'W'
            {
                if (File.Exists(path + "_s"))
                    File.Delete(path + "_s");
                if (!File.Exists(path))
                    return;
                b1 = new Bitmap(path);
                Bitmap resized = new Bitmap(b1, new Size(w, h));
                resized.Save(path + "_s");
                Thread.Sleep(10);
                resized.Dispose();
                b1.Dispose();
                Debug("Resized Succesfully. Remember to use " + path + "_s instead of " + path + " while loading");
            }
            public void DisplayImg(int index) //Displays the image of index 'index'
            {
                Console.Write(image.ElementAt(index));
                Console.Write("\n");
            }

            public int LoadImg(string path) //Loads the image from 'path' and sets it's index
            {
                ConsoleColor bef = Console.ForegroundColor;

                //Display debugs
                Console.ForegroundColor = ConsoleColor.DarkGray;
                if (!File.Exists(path))
                {
                    Console.Write("Error: File not found \n");
                    Console.ForegroundColor = bef;
                    return -1;
                }
                b1 = new Bitmap(path);
                Console.WriteLine("Bitmap loaded from memory. Converting...");
                
                float progress;
                int passed = 0;
                for (int x = 0; x < b1.Height; x++)
                {
                    for (int y = 0; y < b1.Width; y++)
                    {
                        passed++;
                        progress = (float)passed / (b1.Width * b1.Height);
                        bar.progress = progress;
                        bar.Generate();
                        bar.Draw();
                        float greyscaleColor = (b1.GetPixel(y, x).R + b1.GetPixel(y, x).G + b1.GetPixel(y, x).B) / 3;
                        float pixelColor = (float)greyscaleColor / 255f;

                        if (pixelColor >= 0 && pixelColor < 0.2f)
                            data = data + "██";
                        if (pixelColor >= 0.2f && pixelColor < 0.4f)
                            data = data + "▓▓";
                        if (pixelColor >= 0.4f && pixelColor < 0.8f)
                            data = data + "░░";
                        if (pixelColor >= 0.8f && pixelColor <= 1f)                          
                        data = data + "  ";
                    }
                    data += "\n";
                }

                image.Add(data);
                int ai = image.IndexOf(data);
                Console.Write("Done ["+ai + "] \n");
                Console.ForegroundColor = bef;
                data = "";
                b1.Dispose();
                Console.Write("\n");
                return ai;
            }

            public int LoadImgColor(string path) //Loads image from 'path' and sets it's index
            {
                
                string colorData = "";
                ConsoleColor bef = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                if (!File.Exists(path))
                {

                    Console.Write("Error: File not found \n");
                    Console.ForegroundColor = bef;
                    return -1;
                }
                b1 = new Bitmap(path);
                Console.WriteLine("Bitmap loaded from memory. Converting & assigning colors...");
                float progress = 0;
                int passed = 0;
                for (int y = 0; y < b1.Height; y++)
                {
                    for (int x = 0; x < b1.Width; x++)
                    {
                        Color p = b1.GetPixel(x,y);
                        colorData += (ClosestConsoleColor(p.R, p.G, p.B) + ",");
                        Thread.Sleep(0);
                        passed++;
                        progress = (float)passed / (b1.Width * b1.Height);
                        bar.progress = progress;
                        bar.Generate();
                        bar.Draw();

                    }
                    colorData += "x,";
                }
                colors.Add(colorData);
                b1.Dispose();
                Console.Write("Done[" + colors.IndexOf(colorData) + "] \n");
                Console.ForegroundColor = bef;
                return colors.IndexOf(colorData);
            }
            public void DisplayImgColor(int id) //Displays image in color
            {
                ConsoleColor bef = Console.ForegroundColor;
                string image = colors.ElementAt(id);
                string[] ci;
                ci = image.Split(',');
                for (int y = 0; y < ci.Length; y++)
                {
                   if(ci[y] != " " && ci[y] != "" && ci[y] !=null)
                    {
                        if (ci[y] == "x")
                        {
                            Console.Write("\n");
                        }
                        else
                        {
                            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ci[y], true);
                            Console.Write("▓▓");
                            
                        }
                    }

                }
                Console.ForegroundColor = bef;
                Console.Write("\n");
            }
        }
        
    }
}
