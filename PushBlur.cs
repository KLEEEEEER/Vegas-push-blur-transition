﻿using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Push_Blur_Transition
{
    class PushBlur
    {
        private List<TrackEvent> selectedMedias;
        private Vegas myVegas;
        private TransitionMode transitionMode;
        TrackEventGroup group1;
        TrackEventGroup group2;

        public PushBlur(List<TrackEvent> SelectedMedias, Vegas vegas, TransitionMode TransitionMode)
        {
            selectedMedias = SelectedMedias;
            myVegas = vegas;
            transitionMode = TransitionMode;

            group1 = selectedMedias[0].Group;
            group2 = selectedMedias[1].Group;

            if (Config.DebugMode)
            {
                MessageBox.Show($"group1 events = {group1.Count}");
                MessageBox.Show($"group2 events = {group2.Count}");
            }
        }

        public void applyEffects()
        {
            List<TrackEvent> firstGroupCuts = CutFirstGroup();
            AddEffectsFirstGroupCuts(firstGroupCuts);
            ChangeTrackSecondGroup();
            MoveSecondGroup(firstGroupCuts);
            List<TrackEvent> secondGroupCuts = CutSecondGroup();
            AddEffectsSecondGroupCuts(secondGroupCuts);
        }

        public List<TrackEvent> CutFirstGroup()
        {
            List<TrackEvent> trackEvents = new List<TrackEvent>();

            List<TrackEvent> trackEventsInGroup = new List<TrackEvent>();
            foreach (TrackEvent trackEvent in group1)
            {
                trackEventsInGroup.Add(trackEvent);
            }

            foreach (TrackEvent trackEvent in trackEventsInGroup)
            {
                Timecode offset = trackEvent.Length - new Timecode(Config.splitOffset);
                TrackEvent cut = null;
                using (UndoBlock undo = new UndoBlock("Cut first group track events"))
                {
                    cut = trackEvent.Split(offset);
                }
                if (cut != null)
                    trackEvents.Add(cut);
            }
            return trackEvents;
        }

        public void AddEffectsFirstGroupCuts(List<TrackEvent> cuts)
        {
            foreach (TrackEvent trackEvent in cuts)
            {
                if (trackEvent.MediaType == MediaType.Video)
                {
                    VideoEvent videoEvent = trackEvent as VideoEvent;
                    PlugInNode linearBlur = VegasPans.GetPlugin(myVegas, Config.pluginName);
                    using (UndoBlock undo = new UndoBlock("Add plugin to first clip"))
                    {
                        Effect effect = videoEvent.Effects.AddEffect(linearBlur);
                        effect.Preset = Config.pluginPreset;
                    }

                    var method = typeof(VegasPans).GetMethod(VegasPans.GetPanFunction(transitionMode, TransitionVideo.First));
                    var result = method.Invoke(null, new object[] { myVegas, videoEvent });
                }
            }
        }

        public void ChangeTrackSecondGroup()
        {
            foreach (TrackEvent trackEvent in group2)
            {
                if (trackEvent.MediaType == MediaType.Audio)
                {
                    List<Track> audioTracks = VegasHelper.FindAudioTracks(myVegas);
                    bool moved = false;
                    foreach (Track track in audioTracks)
                    {
                        //if (trackEvent.Track == track) continue;
                        if (!isTrackUnique(trackEvent.Track, track, group1)) continue;
                        if (track.MediaType == MediaType.Audio)
                        {
                            using (UndoBlock undo = new UndoBlock("Change track of audio"))
                            {
                                trackEvent.Track = track;
                            }
                            moved = true;
                            break;
                        }   
                    }

                    if (!moved)
                    {
                        AudioTrack newAudioTrack = null;
                        using (UndoBlock undo = new UndoBlock("Creating new AudioTrack"))
                        {
                            newAudioTrack = myVegas.Project.AddAudioTrack();
                        }
                        if (newAudioTrack != null)
                        {
                            using (UndoBlock undo = new UndoBlock("Changed audio track"))
                            {
                                trackEvent.Track = newAudioTrack;
                            }
                        }
                    }
                }
                else if (trackEvent.MediaType == MediaType.Video)
                {
                    List<Track> videoTracks = VegasHelper.FindVideoTracks(myVegas);
                    bool moved = false;
                    foreach (Track track in videoTracks)
                    {
                        if (!isTrackUnique(trackEvent.Track, track, group1)) continue;
                        if (track.MediaType == MediaType.Video)
                        {
                            using (UndoBlock undo = new UndoBlock("Change track of video"))
                            {
                                trackEvent.Track = track;
                            }
                            moved = true;
                            break;
                        }
                    }

                    if (!moved)
                    {
                        VideoTrack newVideoTrack = null;
                        using (UndoBlock undo = new UndoBlock("Creating new AudioTrack"))
                        {
                            newVideoTrack = myVegas.Project.AddVideoTrack();
                        }
                        if (newVideoTrack != null) 
                        {
                            using (UndoBlock undo = new UndoBlock("Changed video track"))
                            {
                                trackEvent.Track = newVideoTrack;
                            }
                        }
                    }
                }
            }
        }

        private bool isTrackUnique(Track initialTrack, Track comparisonTrack, TrackEventGroup trackEventsGroup)
        {
            if (comparisonTrack == initialTrack)
            {
                return false;
            }

            foreach (TrackEvent trackEvent in trackEventsGroup)
            {
                if (trackEvent.Track == comparisonTrack) return false;
            }

            return true;
        }

        public void MoveSecondGroup(List<TrackEvent> cuts)
        {
            if (cuts.Count == 0) return;

            Timecode firstGroupCutStart = cuts[0].Start;
            foreach (TrackEvent trackEvent in group2)
            {
                using (UndoBlock undo = new UndoBlock("Move second group track event"))
                {
                    trackEvent.Start = firstGroupCutStart;
                }
            }
        }

        public List<TrackEvent> CutSecondGroup()
        {
            Timecode offset = new Timecode(Config.splitOffset);
            List<TrackEvent> trackEvents = new List<TrackEvent>();

            List<TrackEvent> trackEventsInGroup = new List<TrackEvent>();
            foreach (TrackEvent trackEvent in group2)
            {
                trackEventsInGroup.Add(trackEvent);
            }

            foreach (TrackEvent trackEvent in trackEventsInGroup)
            {
                TrackEvent cut = null;
                using (UndoBlock undo = new UndoBlock("Cut second group track events"))
                {
                    cut = trackEvent.Split(offset);
                }
                if (cut != null)
                {
                    trackEvents.Add(cut);
                }
            }
            return trackEventsInGroup;
        }

        public void AddEffectsSecondGroupCuts(List<TrackEvent> cuts)
        {
            foreach (TrackEvent trackEvent in cuts)
            {
                if (trackEvent.MediaType == MediaType.Video)
                {
                    VideoEvent videoEvent = trackEvent as VideoEvent;
                    PlugInNode linearBlur = VegasPans.GetPlugin(myVegas, Config.pluginName);
                    using (UndoBlock undo = new UndoBlock("Add plugin to second group cut"))
                    {
                        Effect effect = videoEvent.Effects.AddEffect(linearBlur);
                        effect.Preset = Config.pluginPreset;
                    }

                    var method = typeof(VegasPans).GetMethod(VegasPans.GetPanFunction(transitionMode, TransitionVideo.Last));
                    var result = method.Invoke(null, new object[] { myVegas, videoEvent });
                }
            }
        }
    }
}
