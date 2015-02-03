using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    /// <summary>
    /// Use to track and clean up after unmanaged resources. MUST be wrapped in a using{} clause. Not thread safe. Use only within a single thread.
    /// </summary>
    public class TrackingScope : ITrackingScope
    {
        bool disposed = false;

        public void Dispose()
        {
            if (!CleanAndReturnSkipped().IsEmpty) throw new InvalidOperationException("You must call CleanAndReturnSkipped if you Skip() any resources within a TrackingScope");
        }

        class TrackingInfo
        {
            internal bool skip_cleanup = false;
            internal uint caller_refs = 0;
            internal List<ITrackable> inbound_refs = new List<ITrackable>();
            internal List<ITrackable> outbound_refs = new List<ITrackable>();
            internal long combined_count { get { return caller_refs + inbound_refs.Count + outbound_refs.Count; } }
        }
        private Dictionary<ITrackable, TrackingInfo> info = new Dictionary<ITrackable, TrackingInfo>();

        private TrackingInfo this[ITrackable obj]
        {
            get
            {
                TrackingInfo i;
                if (!info.TryGetValue(obj, out i))
                {
                    i = new TrackingInfo();
                    info[obj] = i;
                }
                if (obj.TrackingScope == null) obj.TrackingScope = this;
                else if (obj.TrackingScope != this)
                {
                    throw new InvalidOperationException("You cannot have dependencies across multiple TrackingScopes or move ITrackable objects between ITrackingScopes except via CleanAndReturnSkipped(). Do not modify .TrackingScope yourself.");
                }
                return i;
            }
        }
        
        private void try_cleanup(ITrackable t, bool ignoreCallerRefs)
        {
            var inf = this[t];
            if (inf.skip_cleanup ||
                inf.inbound_refs.Count > 0 ||
                (inf.caller_refs > 0 && !ignoreCallerRefs)) return;

            //Release resources; there are no inbound refs
            t.ReleaseResources();

            foreach (var other in inf.outbound_refs)
            {
                this[other].inbound_refs.Remove(t);
            }
            var to_try = inf.outbound_refs.ToArray();
            inf.outbound_refs.Clear();
            //With no inbound refs and no outbound refs, we can drop the structure
            info.Remove(t);
            t.TrackingScope = null;

            //Now let's try to clean up the things we used to reference
            foreach (var potential in to_try)
            {
                try_cleanup(potential, ignoreCallerRefs);
            }
            
        }
        private void clean()
        {
            var no_inbound_refs = info.Where((pair) => pair.Value.inbound_refs.Count == 0).ToArray();
            foreach (var loose_end in no_inbound_refs)
            {
                try_cleanup(loose_end.Key, true);
            }
            
        }
        public ITrackingScope CleanAndReturnSkipped()
        {
            clean();
            var ts = new TrackingScope();
            ts.info = this.info;
            this.info = new Dictionary<ITrackable,TrackingInfo>();
            //Fix TrackingScope props
            foreach (var t in ts.info.Keys)
            {
                if (t.TrackingScope != this) throw new InvalidOperationException("You cannot  move ITrackable objects between ITrackingScopes except via CleanAndReturnSkipped(). Do not modify .TrackingScope yourself.");
                t.TrackingScope = ts;
            }
            return ts;
        }


        private bool HasIndirectDependency(ITrackable target, ITrackable dependency)
        {
            if (target == dependency || this[target].outbound_refs.Contains(dependency))
            {
                return true;
            }
            else
            {
                return this[target].outbound_refs.Any((t) => HasIndirectDependency(t, dependency));
            }
        }
  

        public void TrackDependency(ITrackable target, ITrackable dependency)
        {
            if (HasIndirectDependency(dependency, target)) throw new InvalidOperationException("Circular references prohibited.");
            this[target].outbound_refs.Add(dependency);
            this[dependency].inbound_refs.Add(target);
        }
        
        public T Reference<T>(T obj) where T : ITrackable
        {
            this[obj].caller_refs++;
            return obj;
        }

        public T Unreference<T>(T obj) where T : ITrackable
        {
            this[obj].caller_refs--;
            try_cleanup(obj,false);
            return obj;
        }

        public bool IsEmpty
        {
            get { return info.Count == 0; }
        }

        public void Skip(ITrackable obj)
        {
            this[obj].skip_cleanup = true;
        }

        public bool IsSkipped(ITrackable obj)
        {
            return this[obj].skip_cleanup;
        }
    }
}

