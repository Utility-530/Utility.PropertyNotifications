using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Simulation.Infrastructure
{
    public class TimeTrack
    {
        private readonly DateTime start;
        private readonly DateTime end;
        private DateTime current;
        private readonly DateTime[] chapters;

        public TimeTrack(DateTime start, DateTime end, DateTime? current = default, DateTime[] chapters = default)
        {
            if (start > end)
            {
                throw new Exception("fdsfsdf");
            }
            if (current != null && current > end)
            {
                throw new Exception("f55dsf sdf");
            }
            if (current != null && current < start)
            {
                throw new Exception("f 34455dsf sdf");
            }

            this.start = start;
            this.end = end;
            this.current = current ?? start;
            this.chapters = chapters ?? new[] { start, end };
        }

        //public DateTime Previous(TimeSpan timeSpan)
        //{
        //    var tempCurrent = current - timeSpan;
        //    if (tempCurrent < start)
        //        throw new Exception("sdfsdf");
        //    return current = tempCurrent;
        //}

        public DateTime Move(TimeSpan timeSpan)
        {
            var tempCurrent = current + timeSpan;
            if (tempCurrent > End)
                throw new Exception("sdfddsdf");
            return current = tempCurrent;
        }

        public DateTime Move(int skip = 1)
        {
            if (skip >= 0)
            {
                // find next chapter
                var ss = chapters.Where(a => a > current);
                var aa = ss.Skip(skip - 1);
                var chapter = aa.FirstOrDefault();
                if (chapter != default)
                    return current = chapter;
                return current = chapters.Last();
            }
            else
            {     // find next chapter
                var ss = chapters.Where(a => a < current);
                var aa = ss.Skip(skip + 1);
                var chapter = aa.LastOrDefault();
                if (chapter != default)
                    return current = chapter;
                return current = chapters.First();

            }
        }

        //public bool CanMovePrevious(TimeSpan timeSpan)
        //{
        //    var tempCurrent = current - timeSpan;
        //    return (tempCurrent >= start);
        //}
        public bool? CanMove(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
                return null;
            var tempCurrent = current + timeSpan;
            return tempCurrent <= End && tempCurrent >= Start;
        }

        public DateTime Current => current;

        public TimeSpan TimeRemaining => End - current;

        public TimeSpan TimePassed => current - Start;

        public DateTime End => end;

        public DateTime Start => start;

        public void MoveToStart()
        {
            current = Start;
        }

        public void MoveToEnd()
        {
            current = End;
        }

    }


}
