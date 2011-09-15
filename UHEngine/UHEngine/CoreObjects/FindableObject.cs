using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using UHEngine.UI;
using UHEngine.ScreenManagement;

namespace UHEngine.CoreObjects
{
    public class FindableObject : StaticModel, IActionable
    {
        bool isFound;
        
        public string GermanName { get; set; }
        public Texture2D Image { get; set; }
        public Song SoundByte { get; set; }

        public bool IsFound
        {
            get { return isFound; }
            set
            {
                isFound = value;
                if (UIItem != null)
                    UIItem.Found = value;
            }

        }

        public Action Action
        {
            get { return UIItem.OurAction; }
            set { UIItem.OurAction = value; }
        }

        public UIItem UIItem { get; set; }

        public FindableObject(Model model, Vector3 position)
            :base(model, position)
        {
            IsFound = false;
            if (model.Tag != null)
            {
                Name = ((Dictionary<string, object>)model.Tag)["Name"].ToString();
                Image = ScreenManager.Game.Content.Load<Texture2D>(@"UI\objects\" + Name + "_icon");

                bool soundLoaded = false;
                //foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "\\Content\\Sounds"))
                //{
                //    FileInfo fileInfo = new FileInfo(file);
                //    string fileName = fileInfo.Name.Split('.')[0];
                //    string[] translate = fileName.Split('_');
                //    if (translate[1] == Name)
                //    {
                //        SoundByte = ScreenManager.Game.Content.Load<Song>(@"Sounds\" + fileName);
                //        GermanName = translate[0];
                //        soundLoaded = true;
                //        break;
                //    }
                //}

                //if (!soundLoaded)
                //{
                //    throw new Exception("No sound loaded for " + Name);
                //}
            }
        }

    }
}
