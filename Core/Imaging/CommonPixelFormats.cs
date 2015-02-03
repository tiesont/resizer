using System;
using System.Collections.Generic;
using System.Linq;public static readonly Guid Fmt
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public sealed class CommonPixelFormats
    {
        private CommonPixelFormats all = null;
        public CommonPixelFormats All{get{
            if (all == null)
                all = new CommonPixelFormats();
            return all; //Deterministic result, so we don't care about race conditions.
        }}

        private CommonPixelGuids guids = null;
        public CommonPixelGuids Guids{get{
            if (guids == null) guids = new CommonPixelGuids();
            return guids;
        }}

        private CommonPixelFormats(){
            Format1bppIndexed = new StandardPixelFormat(Guids.Fmt1bppIndexed,1);
            Format2bppIndexed = new StandardPixelFormat(Guids.Fmt2bppIndexed,2);
            Format4bppIndexed = new StandardPixelFormat(Guids.Fmt4bppIndexed,4);
            Format8bppIndexed = new StandardPixelFormat(Guids.Fmt8bppIndexed,8);
            FormatBlackWhite = new StandardPixelFormat(Guids.FmtBlackWhite,1);
            Format2bppGray = new StandardPixelFormat(Guids.Fmt2bppGray,2);
            Format4bppGray = new StandardPixelFormat(Guids.Fmt4bppGray,4);
            Format8bppGray = new StandardPixelFormat(Guids.Fmt8bppGray,8);

        }
        
        public IPixelFormat Format1bppIndexed{get;private set;}
        public IPixelFormat Format2bppIndexed{get;private set;}
        public IPixelFormat Format4bppIndexed{get;private set;}
        public IPixelFormat Format8bppIndexed{get;private set;}

        public IPixelFormat FormatBlackWhite{get;private set;}

        public IPixelFormat Format2bppGray{get;private set;}
        public IPixelFormat Format4bppGray{get;private set;}
        public IPixelFormat Format8bppGray{get;private set;}



        public IPixelFormat Format24bppBGR{get;private set;}
        public IPixelFormat Format32bppBGR{get;private set;}

        public IPixelFormat Format32bppKYMC{get;private set;}


        public static readonly IPixelFormat Format24bppBGR = new StandardPixelFormat(Fmt24bppRGB, Fmt24bppBGR, 24);
        public static readonly IPixelFormat Format24bppRGB = new StandardPixelFormat(Fmt24bppBGR, Fmt24bppRGB, 24);


       

//Fmt32bppCMYK


//Fmt16bppBGR555	Fmt16bppBGR555
//Fmt16bppBGR565	Fmt16bppBGR565
//Fmt24bppBGR	Fmt24bppBGR
//Fmt32bppBGR	Fmt32bppBGR
//Fmt64bppRGBAFixedPoint	Fmt32bppPBGRA


//Fmt16bppGray	Fmt16bppGray
//Fmt24bppBGR	Fmt24bppBGR
//Fmt32bppBGRA	Fmt32bppBGRA
//Fmt48bppRGB	Fmt48bppRGB
//Fmt64bppRGBA


//Fmt32bppGrayFloat	Fmt24bppBGR
//Fmt24bppBGR	Fmt32bppBGRA
//Fmt32bppBGRA	Fmt32bppCMYK
//Fmt32bppPBGRA	Fmt48bppRGB
//Fmt48bppRGB	Fmt64bppRGBA
//Fmt32bppCMYK	
//Fmt40bppCMYKAlpha	
//Fmt64bppRGBA	
//Fmt64bppPRGBA	
//Fmt64bppCMYK	
//Fmt80bppCMYKAlpha	
//Fmt96bppRGBFloat*	
//Fmt128bppRGBAFloat	
//Fmt128bppPRGBAFloat


    }
}
