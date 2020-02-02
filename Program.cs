using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace dcitysim
{
    class Program
    {
        //Class that handles the functions
        public class ProgressBar
        {
            public class ProgressBarStyle
            { 
                public char open; 
                public char close; 
                public char progress;
                public char noprogress;
                public char tip;

                public ProgressBarStyle(char open, char close, char progress, char tip,char noprogress)
                {
                    this.open = open;
                    this.close = close;
                    this.progress = progress;
                    this.noprogress = noprogress;
                    this.tip = tip;
                }
            }
            //Variables
            public float progress;
            public ProgressBarStyle style;
            public int width;
            private string graphic;
            private int rp = 0;
            public ProgressBar(float progress, ProgressBarStyle style, int width)
            {
                this.progress = progress;
                this.style = style;
                this.width = width;
            }

            //Functions
            public void Generate()
            {
                string bar = style.open.ToString();
                int pr = (int)Math.Round(progress * width);
                int np = width - pr;
                for(int i = 0; i<width; i++)
                {
                    if(i<pr)
                        bar = bar + style.progress;
                    if (i == pr)
                        bar = bar + style.tip;
                    if(i>pr)
                        bar = bar + style.noprogress;
                }
                bar = bar + style.close + " " + Math.Floor(progress * 100) + "%";
                
                graphic = bar;
                
            }
            
            public void Draw()
            {
                rp = Console.CursorLeft;
                Console.Write(graphic);
                if (progress != 1)
                    Console.CursorLeft = rp;
                else
                    Console.Write(" ");
                
            }
        }

        public class FIMG
        {
            public int grayscale_data;
            public int color_data;

            public FIMG(int grayscale_data, int color_data)
            {
                this.grayscale_data = grayscale_data;
                this.color_data = color_data;
            }
        }
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
                debug("Resized Succesfully. Remember to use " + path + "_s instead of " + path + " while loading");               
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
                debug("Resized Succesfully. Remember to use " + path + "_s instead of " + path + " while loading");
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
                Console.Write("Bitmap loaded from memory. Converting & assigning colors...");
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

        
        //weird code from the web, hope it works but i have no idea how [no it doesn't]

        const int STD_OUT_HANDLE = -11;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int SetConsoleFont(
        IntPtr hOut,
        uint dwFontNum
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int dwType);

        private static Imaging img;
        private static bool advancedMode;
        private static List<FIMG> files;

        //Main
        static void Main(string[] args)
        {
            advancedMode = true;
            Console.SetBufferSize(1920, 1080);
            img = new Imaging();
            files = new List<FIMG>();
            if (args.Length != 0)
            {
                img.LoadImg(args[0]);
                img.DisplayImg(0);
            }
            
            while (true)
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
                Command(command, arg1, arg2);
                
            }
            
        }
        public static void Command(string command, string arg1, string arg2)
        {
            if (advancedMode)
            {
                if (command == "load")
                {
                    if (arg1 != "")
                        img.LoadImg(arg1);
                    else
                        debug("Arguments: path");

                }
                if (command == "loadc")
                {
                    if (arg1 != "")
                        img.LoadImgColor(arg1);
                    else
                        debug("Arguments: path");

                }
                if (command == "res")
                {
                    if (arg1 != "" && arg2 != "")
                        img.Resize(arg1, Int32.Parse(arg2));
                    else
                        debug("Arguments: path , width [recommended is 64, dont increase above 256] ");
                }
                if (command == "display")
                {
                    if (arg1 != "")
                        img.DisplayImg(Int32.Parse(arg1));
                    else
                        debug("Arguments: imgId");
                }
                if (command == "displayc")
                {
                    if (arg1 != "")
                        img.DisplayImgColor(Int32.Parse(arg1));
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
            }
            else
            {
                if(command == "get")
                {

                    if (arg1 != "" && arg2 != "")
                    {
                        if (arg1.StartsWith("http://") || arg1.StartsWith("https://"))
                        {
                            try
                            {
                                using (WebClient client = new WebClient())
                                {
                                    if (File.Exists(arg2)) File.Delete(arg2);
                                    Thread.Sleep(100);
                                    client.DownloadFile(new Uri(arg1), arg2);
                                    client.Dispose();
                                }
                            }
                            catch { debug("URL is incorrect");}
                            finally { debug("Done. Filename: " + arg2 + " , any command works"); }
                        }
                        else
                        {
                            try
                            {
                                if (!File.Exists(arg1)) { debug("File not found"); }
                                else
                                {
                                    if (File.Exists(arg2)) File.Delete(arg2);
                                    File.Copy(arg1, arg2);
                                }
                            }
                            catch { debug("Something went wrong."); }
                            finally { debug("Done.Filename: " + arg2 + ", any command works"); }
                        }
                        
                    }
                    else
                        debug("Arguments: url,name");
                }
                if(command == "You")
                {
                    img.DisplayImgColor(0);
                }
                if(command == "resize")
                {
                    if(arg1 != "" && arg2 != "")
                    {
                        if (arg2.Contains("x"))
                        {
                            string[] p = arg2.Split('x');
                            int w = int.Parse(p[0].Replace("x",""));
                            int h = int.Parse(p[1].Replace("x", ""));
                            img.FResize(arg1, w, h);
                        }
                        else
                        {
                            img.Resize(arg1,int.Parse(arg2));
                        }
                    }
                }
                if(command == "load")
                {
                    if (arg1 != "")
                    {
                        debug("Loading image " + arg1);
                        Thread.Sleep(200);
                        debug("Load grayscale... \n");
                        int gd = img.LoadImg(arg1);
                        debug("Load color... \n");
                        int cd = img.LoadImgColor(arg1);
                        debug("Assigning in array...");
                        Thread.Sleep(1000);
                        files.Add(new FIMG(gd, cd));
                        debug($"Image loaded succesfully with id of {files.Count-1} \n");
                    }
                }

                if(command == "display")
                {
                    if(arg1 != "" && arg2 != "")
                    {
                        int fimgId = int.Parse(arg1);
                        int bw = files[fimgId].grayscale_data;
                        int cd = files[fimgId].color_data;
                        if (arg2 == "grays")
                            img.DisplayImg(bw);
                        else if (arg2 == "color")
                            img.DisplayImgColor(cd);
                        else
                            debug("Arguments: imgId, grays|color");
                    }
                    else
                        debug("Arguments: imgId, grays|color");
                }
            }
            if(command == "advanced")
            {
                if (arg1 != "")
                {
                    bool res;
                    res = bool.TryParse(arg1, out advancedMode);
                    if (!res)
                        debug("Error. Arguments: true|false");
                }
                else
                    debug("Arguments: true|false");
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
            if (command == "cls")
            {
                Console.Clear();
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

        public static void debug(string t)
        {
            ConsoleColor bef = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(t);
            Console.ForegroundColor = bef;
        }
        
    }
}
