using PdfSharp.Fonts; 
using System;
using System.IO;

namespace Presentation_Layer 
{
    public class CustomFontResolver : IFontResolver
    {
       
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Handle Arial font family
            if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
            {
                if (isBold && isItalic) return new FontResolverInfo("Arial#BoldItalic"); 
                if (isItalic) return new FontResolverInfo("Arial#Italic");               
                return new FontResolverInfo("Arial#Regular");                           
            }
            // Handle Times New Roman font family
            else if (familyName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase))
            {
                if (isBold && isItalic) return new FontResolverInfo("Times New Roman#BoldItalic"); 
                if (isBold) return new FontResolverInfo("Times New Roman#Bold");                   
                if (isItalic) return new FontResolverInfo("Times New Roman#Italic");              
                return new FontResolverInfo("Times New Roman#Regular");                           
            }
           

            return null; 
        }

        /// <summary>
        /// Gets the font data (bytes) for a given typeface name.
        /// This method is called by PdfSharp to actually load the font file.
        /// The GetFont method's signature (return type byte[], parameter string) remains the same.
        /// </summary>
        public byte[] GetFont(string faceName)
        {
            try
            {
                string fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

                switch (faceName)
                {
                    case "Arial#Regular":
                        return File.ReadAllBytes(Path.Combine(fontsFolder, "arial.ttf"));
                    case "Arial#Bold":
                        return File.ReadAllBytes(Path.Combine(fontsFolder, "arialbd.ttf"));
                    case "Arial#Italic":
                        return File.ReadAllBytes(Path.Combine(fontsFolder, "ariali.ttf"));
                    case "Arial#BoldItalic":
                        return File.ReadAllBytes(Path.Combine(fontsFolder, "arialbi.ttf"));

                    case "Times New Roman#Regular":
                        return File.ReadAllBytes(Path.Combine(fontsFolder, "times.ttf"));
                    case "Times New Roman#Bold":
                        return File.ReadAllBytes(Path.Combine(fontsFolder, "timesbd.ttf"));
                    case "Times New Roman#Italic":
                        return File.ReadAllBytes(Path.Combine(fontsFolder, "timesi.ttf"));
                    case "Times New Roman#BoldItalic":
                       
                        return File.ReadAllBytes(Path.Combine(fontsFolder, "timesbi.ttf"));

                    default:
                        Console.WriteLine($"WARNING: No font file explicitly mapped for faceName '{faceName}'.");
                        return null; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to load font '{faceName}'. Details: {ex.Message}");
                
                return null;
            }
        }
    }
}