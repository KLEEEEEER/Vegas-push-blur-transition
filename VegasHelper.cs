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
        /*private static double splitOffset = 150;
        private static string pluginName = "VEGAS Linear Blur";
        private static string pluginPreset = "Horizontal Light";*/

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

        /*public static void applyEffects(List<TrackEvent> selectedMedias, Vegas vegas, TransitionMode transitionMode)
        {
            // https://forums.creativecow.net/docs/forums/post.php?forumid=24&postid=880218&univpostid=880218&pview=t
            *//*
             * Here is what has changed between scripts and commands. Vegas wraps your entire script in an undo block. It can do this because all processing stops while the script is run. Because custom commands are always available as an extension while the project is being work on, you must declare an undo block yourself in your code whenever you change the project.
             * */
            /*for (int i = 0; i < selectedMedias.Count; i++)
            {
                MessageBox.Show($"Start={selectedMedias[i].Start.ToMilliseconds()}");
            }*//*
            
            try
            {
                *//* *
                 * 1. Обрезать первый клип
                 * 1.1 Добавить эффекты и пан на первый обрезанный клип
                 * 2. Переместить по возможности второй клип на другую дорожку
                 * 3. Переместить второй клип на точку начала обрезанного фрагмента из первого клипа
                 * 4. Обрезать начало второго клипа
                 * 4.2 Добавить эффекты и пан на второй обрезанный клип
                 *//*

                TrackEvent firstCut1 = null;
                TrackEvent firstCut2 = null;
                Timecode firstOffset = selectedMedias[0].Length - new Timecode(splitOffset);

                using (UndoBlock undo = new UndoBlock("Cut first clip"))
                {
                    firstCut1 = selectedMedias[0].Split(firstOffset);
                }
                using (UndoBlock undo = new UndoBlock("Cut first clip"))
                {
                    firstCut2 = selectedMedias[1].Split(firstOffset);
                }

                VideoEvent videoEvent = null;

                if (firstCut1 != null)
                {
                    videoEvent = firstCut1 as VideoEvent;
                } 
                else if (firstCut2 != null)
                {
                    videoEvent = firstCut2 as VideoEvent;
                }

                if (videoEvent != null)
                {
                    PlugInNode linearBlur = GetPlugin(vegas, pluginName);
                    using (UndoBlock undo = new UndoBlock("Add plugin to first clip"))
                    {
                        Effect effect = videoEvent.Effects.AddEffect(linearBlur);
                        effect.Preset = pluginPreset;
                    }

                    var method = typeof(VegasPans).GetMethod(GetPanFunction(transitionMode, TransitionVideo.First));
                    var result = method.Invoke(null, new object[] { vegas, videoEvent });
                }

                if (firstCut1 != null)
                {
                    selectedMedias[2].Start = firstCut1.Start;
                    selectedMedias[3].Start = firstCut1.Start;
                }



                *//*TrackEvent firstCut = null;

                for (int i = 0; i < selectedMedias.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                        case 1:
                            Timecode firstOffset = selectedMedias[i].Length - new Timecode(splitOffset);
                            using (UndoBlock undo = new UndoBlock("Cut first clip"))
                            {
                                firstCut = selectedMedias[i].Split(firstOffset);
                            }
                            break;
                        case 2:
                        case 3:
                            if (firstCut != null)
                            {
                                using (UndoBlock undo = new UndoBlock("Move second clip"))
                                {
                                    selectedMedias[i].Start = firstCut.Start;
                                }
                            }
                            else
                            {
                                MessageBox.Show($"FirstCut is empty!");
                            }


                            Timecode secondOffset = new Timecode(splitOffset);
                            using (UndoBlock undo = new UndoBlock("Cut second clip"))
                            {
                                TrackEvent secondCut = selectedMedias[i].Split(secondOffset);
                            }
                            break;
                    }
                }

                for (int i = 0; i < selectedMedias.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                        case 1:
                            AddFirstVideoEffect(vegas, firstCut, transitionMode);
                            break;
                        case 2:
                        case 3:
                            ChangeSecondVideoTrack(selectedMedias, vegas, i);
                            break;
                    }
                }

                for (int i = 0; i < selectedMedias.Count; i++)
                {
                    switch (i)
                    {
                        case 2:
                        case 3:
                            AddSecondVideoEffect(selectedMedias, vegas, i, transitionMode);
                            break;
                    }
                }*//*
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + '\n' + ex.StackTrace);
            }
        }

        private static void AddSecondVideoEffect(List<TrackEvent> selectedMedias, Vegas vegas, int i, TransitionMode transitionMode)
        {
            if (selectedMedias[i].MediaType == MediaType.Video)
            {
                VideoEvent videoEvent = selectedMedias[i] as VideoEvent;
                PlugInNode linearBlur = GetPlugin(vegas, pluginName);
                using (UndoBlock undo = new UndoBlock("Add plugin to second video"))
                {
                    Effect effect = videoEvent.Effects.AddEffect(linearBlur);
                    effect.Preset = pluginPreset;
                }

                //PanFromLeftToCenter(vegas, videoEvent);
                var method = typeof(VegasPans).GetMethod(GetPanFunction(transitionMode, TransitionVideo.Last));
                var result = method.Invoke(null, new object[] { vegas, videoEvent });
            }
        }

        private static void ChangeSecondVideoTrack(List<TrackEvent> selectedMedias, Vegas vegas, int i)
        {
            if (selectedMedias[i].MediaType == MediaType.Audio)
            {
                List<Track> audioTracks = FindAudioTracks(vegas);
                foreach (Track track in audioTracks)
                {
                    if (selectedMedias[i].Track == track) continue;
                    if (track.MediaType == MediaType.Audio)
                        using (UndoBlock undo = new UndoBlock("Change track of audio"))
                        {
                            selectedMedias[i].Track = track;
                        }
                    break;
                }
            }
            else if (selectedMedias[i].MediaType == MediaType.Video)
            {
                List<Track> videoTracks = FindVideoTracks(vegas);
                foreach (Track track in videoTracks)
                {
                    if (selectedMedias[i].Track == track) continue;
                    if (track.MediaType == MediaType.Video)
                        using (UndoBlock undo = new UndoBlock("Change track of video"))
                        {
                            selectedMedias[i].Track = track;
                        }
                    break;
                }
            }
        }

        private static void AddFirstVideoEffect(Vegas vegas, TrackEvent firstCut, TransitionMode transitionMode)
        {
            if (firstCut != null && firstCut.MediaType == MediaType.Video)
            {
                VideoEvent videoEvent = firstCut as VideoEvent;
                PlugInNode linearBlur = GetPlugin(vegas, pluginName);
                using (UndoBlock undo = new UndoBlock("Add plugin to first clip"))
                {
                    Effect effect = videoEvent.Effects.AddEffect(linearBlur);
                    effect.Preset = pluginPreset;
                }

                //PanFromCenterToRight(vegas, videoEvent);
                var method = typeof(VegasPans).GetMethod(GetPanFunction(transitionMode, TransitionVideo.First));
                var result = method.Invoke(null, new object[] { vegas, videoEvent });
            }
        }

        private static string GetPanFunction(TransitionMode transitionMode, TransitionVideo transitionVideo)
        {
            switch (transitionMode)
            {
                case TransitionMode.ToRight:
                    if (transitionVideo == TransitionVideo.First) return "PanFromCenterToRight";
                    if (transitionVideo == TransitionVideo.Last) return "PanFromLeftToCenter";
                    break;
                case TransitionMode.ToLeft:
                    if (transitionVideo == TransitionVideo.First) return "PanFromRightToCenter";
                    if (transitionVideo == TransitionVideo.Last) return "PanFromCenterToLeft";
                    break;
                default:
                    return String.Empty;
            }
            return String.Empty;
        }*/
    }
}
