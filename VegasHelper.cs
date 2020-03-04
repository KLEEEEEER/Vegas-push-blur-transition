using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Push_Blur_Transition
{
    class VegasHelper
    {
        public static List<TrackEvent> FindSelectedEvents(Project project)
        {
            List<TrackEvent> selectedEvents = new List<TrackEvent>();
            foreach (Track track in project.Tracks)
            {
                foreach (TrackEvent trackEvent in track.Events)
                {
                    if (trackEvent.Selected)
                    {
                        selectedEvents.Add(trackEvent);
                    }
                }
            }
            return selectedEvents;
        }

        public static bool isPickedTwoVideos(List<TrackEvent> selectedMedias)
        {
            if (selectedMedias.Count != 2) return false;
            foreach (TrackEvent trackEvent in selectedMedias)
            {
                if (!trackEvent.IsVideo()) return false;
            }
            return true;
        }

        public static bool isPickedTwoVideosAndTwoAudios(List<TrackEvent> selectedMedias)
        {
            if (selectedMedias.Count != 4) return false;

            List<TrackEvent> videos = new List<TrackEvent>();
            List<TrackEvent> audios = new List<TrackEvent>();
            foreach (TrackEvent trackEvent in selectedMedias)
            {
                if (trackEvent.IsVideo()) 
                {
                    videos.Add(trackEvent);
                } 
                else if(trackEvent.IsAudio())
                {
                    audios.Add(trackEvent);
                }
            }

            return (videos.Count == 2 && audios.Count == 2) ? true : false;
        }

        public static bool isPickedEventsInTheSameGroup(List<TrackEvent> selectedMedias)
        {
            TrackEventGroup group = null;
            foreach (TrackEvent trackEvent in selectedMedias)
            {
                if (group == null)
                {
                    group = trackEvent.Group;
                }
                else
                {
                    if (group == trackEvent.Group)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static List<Track> FindVideoTracks(Vegas vegas)
        {
            List<Track> videoTracks = new List<Track>();
            foreach (Track currentTrack in vegas.Project.Tracks)
            {
                if (currentTrack.IsAudio())
                    continue;
                videoTracks.Add(currentTrack);
            }

            return videoTracks;
        }

        public static List<Track> FindAudioTracks(Vegas vegas)
        {
            List<Track> audioTracks = new List<Track>();
            foreach (Track currentTrack in vegas.Project.Tracks)
            {
                if (currentTrack.IsVideo())
                    continue;
                audioTracks.Add(currentTrack);
            }

            return audioTracks;
        }

        static Effect GetEffect(Vegas vegas, String plugInName)
        {
            PlugInNode plugIn = vegas.PlugIns.GetChildByName(plugInName);
            if (plugIn == null) return null;
            Effect linearBlur = new Effect(plugIn);
            if (null == plugIn)
                throw new ApplicationException(String.Format("Failed to find plug-in: '{0}'", plugInName));
            return linearBlur;
        }

        static PlugInNode GetPlugin(Vegas vegas, String plugInName)
        {
            PlugInNode plugIn = vegas.PlugIns.GetChildByName(plugInName);
            return (plugIn != null) ? plugIn : null;
        }

        public static void testVegasBehaviour(TrackEvent trackEvent)
        {
            try
            {
                //trackEvent.Split(new Timecode(splitOffset));
                trackEvent.Mute = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }
    }
}
