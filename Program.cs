using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace dcitysim
{
    partial class Program
    {
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

       

        private static Imaging img;
        private static bool advancedMode;


        private static List<FIMG> files;

        //Main
        static void Main(string[] args)
        {
            advancedMode = false;
            Console.SetBufferSize(1920, 1080);
            img = new Imaging();
            files = new List<FIMG>();
            if (args.Length != 0)
            {
                img.LoadImg(args[0]);
                img.DisplayImg(0);
            }
            Debug("Welcome to CmdImg v. 0.9");
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
                        Debug("Arguments: path");

                }
                if (command == "loadc")
                {
                    if (arg1 != "")
                        img.LoadImgColor(arg1);
                    else
                        Debug("Arguments: path");

                }
                if (command == "res")
                {
                    if (arg1 != "" && arg2 != "")
                        img.Resize(arg1, Int32.Parse(arg2));
                    else
                        Debug("Arguments: path , width [recommended is 64, dont increase above 256] ");
                }
                if (command == "display")
                {
                    if (arg1 != "")
                        img.DisplayImg(Int32.Parse(arg1));
                    else
                        Debug("Arguments: imgId");
                }
                if (command == "displayc")
                {
                    if (arg1 != "")
                        img.DisplayImgColor(Int32.Parse(arg1));
                    else
                        Debug("Arguments: imgId");
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
                        Debug("Arguments: imgId , path");
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
                        Debug("Arguments: sqcSavePath");
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
                        Debug("Arguments: url");
                }
            }
            else
            {
                // Download/copy file to the work directory
                if (command == "get") 
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
                            catch { Debug("URL is incorrect");}
                            finally { Debug("Done. Filename: " + arg2 + " , any command works"); }
                        }
                        else
                        {
                            try
                            {
                                if (!File.Exists(arg1)) { Debug("File not found"); }
                                else
                                {
                                    if (File.Exists(arg2)) File.Delete(arg2);
                                    File.Copy(arg1, arg2);
                                }
                            }
                            catch { Debug("Something went wrong."); }
                            finally { Debug("Done.Filename: " + arg2 + ", any command works"); }
                        }
                        
                    }
                    else
                        Debug("Arguments: url,name");
                }
                // Resize image to fit
                if(command == "resize")
                {
                    if(arg1 != "" && arg2 != "")
                    {
                        if (Path.IsPathRooted(arg1))
                            Debug("Image not accessible. Try using get command first.");
                        else
                        {
                            if (arg2.Contains("x"))
                            {
                                string[] p = arg2.Split('x');
                                int w = int.Parse(p[0].Replace("x", ""));
                                int h = int.Parse(p[1].Replace("x", ""));
                                img.FResize(arg1, w, h);
                            }
                            else
                            {
                                img.Resize(arg1, int.Parse(arg2));
                            }
                        }
                    }
                }
                // Load image from workspace. Should check for relative path;
                if(command == "load")
                {
                    if (arg1 != "")
                    {
                        if (Path.IsPathRooted(arg1))
                            Debug("Image not accessible. Try using get command first.");
                        else
                        {
                            try
                            {
                                Debug("Loading image " + arg1);
                                Thread.Sleep(200);
                                Debug("Load grayscale... \n");
                                int gd = img.LoadImg(arg1);
                                Debug("Load color... \n");
                                int cd = img.LoadImgColor(arg1);
                                Debug("Assigning in array...");
                                Thread.Sleep(1000);
                                files.Add(new FIMG(gd, cd));

                            }
                            catch { Debug("Something went really wrong. "); }
                            finally { Debug($"Image loaded succesfully with id of {files.Count - 1} \n"); }
                        }
                    }
                }
                // Display image
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
                            Debug("Arguments: imgId, grays|color");
                    }
                    else
                        Debug("Arguments: imgId, grays|color");
                }

                if (command == "help")
                {
                    Debug("Help menu: \n ------------------------------");
                    Debug("get url/path,name - clone file from <url/path> into work directory as <name>");
                    Debug("load workpath - convert file from <workpath> into a displayable graphic");
                    Debug("display id,color|grays - display graphic of id <id> and display it in <color|grays>");
                    Debug("resize workpath,width - resize image from <workpath> to <width> with keeping proportions");
                    Debug("resize workpath,widthxheight - resize image from <workpath> to <widthxheight> specified as [width]x[height]");
                    Debug("advanced true|false - switch advanced mode to <true|false>, <true> enables an old version of this programme with less automation. DON'T DO IF NECESSARY");
                    Debug("help - display this message");
                }
            }
            // Toggle advanced mode
            if(command == "advanced")
            {
                if (arg1 != "")
                {
                    bool res;
                    res = bool.TryParse(arg1, out advancedMode);
                    if (!res)
                        Debug("Error. Arguments: true|false");
                }
                else
                    Debug("Arguments: true|false");
            }
            // Change colors
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
                    Debug("Arguments: default|whiteboard|matrix");

            }
            // Clear console
            if (command == "cls")
            {
                Console.Clear();
            }
            // Change colors
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

        public static void Debug(string t)
        {
            ConsoleColor bef = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(t);
            Console.ForegroundColor = bef;
        }
        
    }
}
