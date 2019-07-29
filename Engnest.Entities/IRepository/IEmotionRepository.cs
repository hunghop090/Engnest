using Engnest.Entities.Entity;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.IRepository
{
	public interface  IEmotionRepository : IDisposable
	{
        List<Emotion> GetEmotions();
        Emotion GetEmotionByID(long EmotionId);
        List<Emotion> GetEmotionByTargetId(long TargetId,long UserId);
        void InsertEmotion(Emotion Emotion);
        void DeleteEmotion(long EmotionID);
        void UpdateEmotion(Emotion Emotion);
        void Save();
	}
}