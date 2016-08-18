using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGitLab;
using NGitLab.Impl;
using NGitLab.Models;

namespace CRBot
{
    public class Generator
    {
        private static readonly Random Random = new Random();

        private readonly string _gitLabUrl;
        private readonly string _projectName;
        private readonly IList<string> _developers;
        private readonly GitLabClient _gitLabClient;
        private readonly API _api;

        public Generator(string gitLabUrl, string gitLabToken, string projectName, IList<string> developers)
        {
            _api = new API(gitLabUrl, gitLabToken);
            _gitLabUrl = gitLabUrl;
            _projectName = projectName;
            _developers = developers;
            _gitLabClient = GitLabClient.Connect(gitLabUrl, gitLabToken);
        }

        public string GenerateMessage()
        {
            var text = new StringBuilder();

            var project = _gitLabClient.Projects.Accessible.Single(x => x.Name == _projectName);

            var mergeRequestClient = _gitLabClient.GetMergeRequest(project.Id);

            var awardEmojiClient = new AwardEmojiClient(_api, project.Id);

            var openedMergeRequests = mergeRequestClient.AllInState(MergeRequestState.opened);

            foreach (var review in openedMergeRequests)
            {
                var awards = awardEmojiClient.GetAwardsForMergeRequest(review.Id).ToList();

                var downvotes = awards.Where(x => x.IsDownvote).ToList();
                var upvotes = awards.Where(x => x.IsUpvote).ToList();

                if (downvotes.Any())
                {
                    text.AppendLine($"{ReviewUrl(project, review)} - still :-1: ({UserRefs(downvotes.Select(x => x.User.Username))})");
                }
                else if (upvotes.Count < 3)
                {
                    var reviewsNeeded = 3 - upvotes.Count;

                    var canReview = _developers
                        .Except(new[] { review.Author.Username })
                        .Except(upvotes.Select(x => x.User.Username));

                    var reviewers = TakeRandom(canReview, Random, reviewsNeeded);

                    text.AppendLine($"{ReviewUrl(project, review)} - {reviewsNeeded} reviews needed ({UserRefs(reviewers)})");
                }
            }

            return text.ToString();
        }

        private string ReviewUrl(Project project, MergeRequest review)
        {
            return $"<{_gitLabUrl}/{project.PathWithNamespace}/merge_requests/{review.Iid}|{review.Title}>";
        }

        private string UserRefs(IEnumerable<string> usernames)
        {
            return string.Join(", ", usernames.Select(x => "@" + x));
        }

        public static IEnumerable<T> TakeRandom<T>(IEnumerable<T> enumerable, Random random, int count)
        {
            var items = enumerable.ToList();

            if (items.Count <= count)
            {
                return items;
            }

            var used = new HashSet<int>();
            var selected = new List<T>();

            int remaining = count;

            while (remaining > 0)
            {
                var idx = random.Next(0, items.Count);

                if (used.Contains(idx))
                {
                    continue;
                }

                selected.Add(items[idx]);
                used.Add(idx);

                remaining--;
            }

            return selected;
        }
    }
}