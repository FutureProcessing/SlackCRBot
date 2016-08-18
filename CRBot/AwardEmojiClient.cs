using System.Collections.Generic;
using NGitLab.Impl;

namespace CRBot
{
    public class AwardEmojiClient
    {
        private readonly API _api;
        private readonly int _projectId;

        public AwardEmojiClient(API api, int projectId)
        {
            _api = api;
            _projectId = projectId;
        }

        public IEnumerable<AwardEmoji> GetAwardsForMergeRequest(int mergeRequestId)
        {
            return _api.Get().GetAll<AwardEmoji>($"projects/{_projectId}/merge_requests/{mergeRequestId}/award_emoji");
        }
    }
}