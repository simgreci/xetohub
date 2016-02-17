﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLXEtoEventHub.XEPosition
{
    public class XEPosition
    {
        public string LastFile { get; set; }
        public long Offset { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is XEPosition))
                throw new ArgumentException(string.Format("Cannot compare a {0:S} with a XEPosition", obj.GetType().FullName));
            XEPosition x2 = (XEPosition)obj;

            return this.LastFile.Equals(x2.LastFile) && this.Offset.Equals(x2.Offset);
        }

        public override int GetHashCode()
        {
            return LastFile.GetHashCode() ^ Offset.GetHashCode();
        }
    }

}