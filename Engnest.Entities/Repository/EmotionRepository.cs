using Engnest.Entities.Common;
using Engnest.Entities.Entity;
using Engnest.Entities.IRepository;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Engnest.Entities.Repository
{
	public class EmotionRepository : IEmotionRepository, IDisposable
    {
        private EngnestContext context;

        public EmotionRepository(EngnestContext context)
        {
            this.context = context;
        }

        public List<Emotion> GetEmotions()
        {
            return context.Emotions.ToList();
        }

        public Emotion GetEmotionByID(long id)
        {
            return context.Emotions.Find(id);
        }

        public List<Emotion> GetEmotionByTargetId(long id)
        {
            return context.Emotions.Where(x => x.TargetId == id).ToList();
        }

        public void InsertEmotion(Emotion Emotion)
        {
            context.Emotions.Add(Emotion);
            Save();
        }

        public void DeleteEmotion(long EmotionID)
        {
            Emotion Emotion = context.Emotions.Find(EmotionID);
            context.Emotions.Remove(Emotion);
			Save();
        }

        public void UpdateEmotion(Emotion Emotion)
        {
            context.Entry(Emotion).State = EntityState.Modified;
			Save();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}