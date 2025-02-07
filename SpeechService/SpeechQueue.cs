﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EddiSpeechService
{
    public class SpeechQueue
    {
        public List<ConcurrentQueue<EddiSpeech>> priorityQueues { get; private set; }
        public bool hasSpeech => priorityQueues.Any(q => q.Count > 0);
        public bool isQueuePaused;

        public SpeechQueue ()
        {
            PrepareSpeechQueues();
        }

        public List<int?> priorities => PreparePrioritiesList();

        private List<int?> PreparePrioritiesList()
        {
            List<int?> result = new List<int?>();
            for (int i = 1; i <= (priorityQueues.Count - 1); i++)
            {
                if (i > 0)
                {
                    result.Add(i);
                }
            }
            return result;
        }

        private void PrepareSpeechQueues()
        {
            priorityQueues = new List<ConcurrentQueue<EddiSpeech>>();

            // Priority 0: System messages (always top priority)
            // Priority 1: Highest user settable priority, interrupts lower priorities
            // Priority 2: High priority
            // Priority 3: Standard priority
            // Priority 4: Low priority
            // Priority 5: Lowest priority, interrupted by any higher priority

            for (int i = 0; i <= 5; i++)
            {
                priorityQueues.Add(new ConcurrentQueue<EddiSpeech>());
            }
        }

        public void Enqueue(EddiSpeech speech)
        {
            if (speech == null) { return; }
            if (priorityQueues.ElementAtOrDefault(speech.priority) != null)
            {
                dequeueStaleSpeech(speech);
                priorityQueues[speech.priority].Enqueue(speech);
            }
        }

        public bool TryDequeue(out EddiSpeech speech)
        {
            speech = null;
            if ( isQueuePaused ) { return false; }
            // ReSharper disable once ForCanBeConvertedToForeach - We want to enforce the priority order
            for ( var i = 0; i < priorityQueues.Count; i++)
            {
                if (priorityQueues[i].TryDequeue(out var selectedSpeech))
                {
                    speech = selectedSpeech;
                    return true;
                }
            }
            return false;
        }

        public bool TryPeek(out EddiSpeech speech)
        {
            speech = null;
            if ( isQueuePaused ) { return false; }
            // ReSharper disable once ForCanBeConvertedToForeach - We want to enforce the priority order
            for ( var i = 0; i < priorityQueues.Count; i++)
            {
                if (priorityQueues[i].TryPeek(out var selectedSpeech))
                {
                    speech = selectedSpeech;
                    return true;
                }
            }
            return false;
        }

        public void DequeueAllSpeech()
        {
            // Don't clear system messages (priority 0)
            for (int i = 1; i < priorityQueues.Count; i++)
            {
                while (priorityQueues[i].TryDequeue(out _)) { }
            }
        }

        public void DequeueSpeechOfType(string type)
        {
            // Don't clear system messages (priority 0)
            for (int i = 1; i < priorityQueues.Count; i++)
            {
                var priorityHolder = new ConcurrentQueue<EddiSpeech>();
                while (priorityQueues[i].TryDequeue(out var eddiSpeech)) { filterSpeechQueue(type, ref priorityHolder, eddiSpeech); };
                while (priorityHolder.TryDequeue(out var eddiSpeech)) { priorityQueues[i].Enqueue(eddiSpeech); };
            }
        }

        public void Pause ()
        {
            isQueuePaused = true;
        }

        public void Unpause ()
        {
            isQueuePaused = false;
        }

        private void filterSpeechQueue(string type, ref ConcurrentQueue<EddiSpeech> speechQueue, EddiSpeech eddiSpeech)
        {
            if (eddiSpeech.eventType != type)
            {
                speechQueue.Enqueue(eddiSpeech);
            }
        }

        private void dequeueStaleSpeech(EddiSpeech eddiSpeech)
        {
            // List EDDI event types of where stale event data should be removed in favor of more recent data
            string[] eventTypes = new string[]
                {
                    "Cargo scoop",
                    "Docking denied",
                    "Docking requested",
                    "Glide",
                    "Hardpoints",
                    "Guidance system",
                    "Heat damage",
                    "Heat warning",
                    "Hull damaged",
                    "Landing gear",
                    "Lights",
                    "Near surface",
                    "Next jump",
                    "Silent running",
                    "SRV turret deployable",
                    "Under attack"
                };

            foreach (string eventType in eventTypes)
            {
                if (eddiSpeech.eventType == eventType)
                {
                    DequeueSpeechOfType(eventType);
                }
            }
        }
    }
}
