using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Net;


namespace dcitysim
{
    class Program
    {
        class Imaging
        {
            public List<string> colors;
            public List<string> image;
            string data;
            Bitmap b1;

            public Imaging()
            {
                image = new List<string>();
                colors = new List<string>();
            }
            public void resize(string path,int w)
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
                debug("Resized Succesfully. Remember to use " + path + "_s instead of" + path + "while loading");
                
                
               
            }
            public void displayImg(int index)
            {
                Console.Write(image.ElementAt(index));
                Console.Write("\n");
            }

            public void loadImg(string path)
            {
                ConsoleColor bef = Console.ForegroundColor;

                //Display debugs
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Loading resource:" + path + "...");

                if (!File.Exists(path))
                {

                    Console.Write("Error: File not found \n");
                    Console.ForegroundColor = bef;
                    return;
                }
                b1 = new Bitmap(path);

                float progress;
                int passed = 0;
                for (int x = 0; x < b1.Height; x++)
                {
                    for (int y = 0; y < b1.Width; y++)
                    {
                        passed++;
                        progress = (float)passed / (b1.Width * b1.Height);
                        Thread.Sleep(0);
                        
                            
                        float greyscaleColor = (b1.GetPixel(y, x).R + b1.GetPixel(y, x).G + b1.GetPixel(y, x).B) / 3;
                        float pixelColor = (float)greyscaleColor / 255f;

                        if (pixelColor >= 0 && pixelColor <= 0.1f)
                            data = data + "██";
                        if (pixelColor >= 0.2f && pixelColor <= 0.4f)
                            data = data + "▓▓";
                        if (pixelColor >= 0.5 && pixelColor <= 0.8)
                            data = data + "░░";
                        if (pixelColor >= 0.9 && pixelColor <= 1)
                           
                        data = data + "  ";

                    }
                    Console.Write(".");
                    data += "\n";
                }

                image.Add(data);
                Console.Write("Done ["+image.IndexOf(data) + "] \n");
                Console.ForegroundColor = bef;
                data = "";
                b1.Dispose();
                Console.Write("\n");
            }

            public void loadImgC(string path)
            {
                
                string colorData ="";
                ConsoleColor bef = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Loading resource:" + path + "..");
                if (!File.Exists(path))
                {

                    Console.Write("Error: File not found \n");
                    Console.ForegroundColor = bef;
                    return;
                }
                b1 = new Bitmap(path);
                for (int y = 0; y < b1.Height; y++)
                {
                    for (int x = 0; x < b1.Width; x++)
                    {
                        Color p = b1.GetPixel(x,y);
                        colorData += (ClosestConsoleColor(p.R, p.G, p.B) + ",");
                        Thread.Sleep(0);
                        

                    }
                    Console.Write(".");
                    colorData += "x,";
                }
                //Console.Write(colorData);
                colors.Add(colorData);
                b1.Dispose();
                Console.Write("Done[" + colors.IndexOf(colorData) + "] \n");
                Console.ForegroundColor = bef;
            }
            public void displayImgC(int id)
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
        static void Main(string[] args)
        {
            Imaging img = new Imaging();
            if (args.Length != 0)
            {
                img.loadImg(args[0]);
                img.displayImg(0);
            }
            
            //Thread.Sleep(1000);
            /*img.loadImg("logo.bmp");
            img.loadImg("mail_1.bmp");
            img.loadImg("mail_2.bmp");*/
            while (1 == 1)
            {
                string cmd = Console.ReadLine();
                string command;
                string arg1;
                string arg2;
                if (cmd.Contains(" "))
                {
                    command = cmd.Split(' ')[0];
                    arg1 = cmd.Split(' ')[1];
                    if (cmd.Split(' ').Length == 3) arg2 = cmd.Split(' ')[2];
                    else arg2 = "";
                }
                else
                {
                    command = cmd;
                    arg1 = "";
                    arg2 = "";
                }
                if (command == "load")
                {
                    if (arg1 != "")
                        img.loadImg(arg1);
                    else
                        debug("Arguments: path");
                        
                }
                if (command == "loadc")
                {
                    if (arg1 != "")
                        img.loadImgC(arg1);
                    else
                        debug("Arguments: path");

                }
                if (command == "res")
                {
                    if (arg1 != "" && arg2 != "")
                        img.resize(arg1, Int32.Parse(arg2));
                    else
                        debug("Arguments: path , width [recommended is 64, dont increase above 256] ");
                }
                if (command == "display")
                {
                    if (arg1 != "")
                        img.displayImg(Int32.Parse(arg1));
                    else
                        debug("Arguments: imgId");
                }
                if (command == "displayc")
                {
                    if (arg1 != "")
                        img.displayImgC(Int32.Parse(arg1));
                    else
                        debug("Arguments: imgId");
                }
                if (command == "sqc")
                {
                    if (arg1 != "" && arg2 != "")
                    {
                        StreamWriter sw = new StreamWriter(arg2);
                        sw.Write(img.image.ElementAt(Int32.Parse(arg1)));
                        sw.Close();
                        Console.Write("  Done! \n");
                    }
                    else
                        debug("Arguments: imgId , path");
                }
                if (command == "rqc")
                {
                    if (arg1 != "")
                    {
                        StreamReader sr = new StreamReader(arg1);
                        Console.WriteLine(sr.ReadToEnd());


                        sr.Close();
                    }
                    else
                        debug("Arguments: sqcSavePath");
                }
                if (command == "get")
                {
                   
                    if (arg1 != "")
                        using (WebClient client = new WebClient())
                        {
                            if (File.Exists("dimg")) File.Delete("dimg");
                            Thread.Sleep(100);
                            client.DownloadFile(new Uri(arg1), "dimg");
                            Console.WriteLine("Download finished. Filename: 'dimg' ,any command works");
                            client.Dispose();




                        }
                    else
                        debug("Arguments: url");
                }
                if (command == "colors")
                {
                    if (arg1 == "whiteboard")
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Clear();
                        
                    }
                    if (arg1 == "default")
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Clear();
                    }
                    if (arg1 == "matrix")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Clear();
                    }
                    if (arg1 == "")
                        debug("Arguments: default|whiteboard|matrix");
                    
                }
                if (command == "color")
                {
                    if (arg1 == "black")
                        Console.ForegroundColor = ConsoleColor.Black;
                    if (arg1 == "white")
                        Console.ForegroundColor = ConsoleColor.White;
                    if (arg1 == "green")
                        Console.ForegroundColor = ConsoleColor.Green;
                    if (arg1 == "red")
                        Console.ForegroundColor = ConsoleColor.Red;
                    if (arg1 == "yellow")
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    if (arg2 == "black")
                        Console.BackgroundColor = ConsoleColor.Black;
                    if (arg2 == "white")
                        Console.BackgroundColor = ConsoleColor.White;
                    if (arg2 == "green")
                        Console.BackgroundColor = ConsoleColor.Green;
                    if (arg2 == "red")
                        Console.BackgroundColor = ConsoleColor.Red;
                    if (arg2 == "yellow")
                        Console.BackgroundColor = ConsoleColor.Yellow;

                    Console.Clear();
                }
            }
            
        }
        public static void debug(string t)
        {
            ConsoleColor bef = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(t);
            Console.ForegroundColor = bef;
        }
        static ConsoleColor ClosestConsoleColor(byte r, byte g, byte b)
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
    }
}
