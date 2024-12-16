using System.Collections.Generic;
using System.Linq;
using Nsu.HackathonProblem.Contracts;

namespace Hackathon
{
    public class SimpleTeamBuildingStrategy : ITeamBuildingStrategy
    {
        public IEnumerable<Team> BuildTeams(
            IEnumerable<Employee> teamLeads,
            IEnumerable<Employee> juniors,
            IEnumerable<Wishlist> teamLeadsWishlists,
            IEnumerable<Wishlist> juniorsWishlists)
        {
            var pairs = new List<Team>();
            var usedJuniors = new HashSet<int>();
            var usedTeamLeads = new HashSet<int>();

            var allPossiblePairs = new List<(Employee junior, Employee teamLead, double happiness)>();

            foreach (var junior in juniors)
            {
                var juniorWishlist = juniorsWishlists.FirstOrDefault(wl => wl.EmployeeId == junior.Id);
                if (juniorWishlist == null) continue;

                foreach (var preferredTeamLeadId in juniorWishlist.DesiredEmployees)
                {
                    var preferredTeamLead = teamLeads.FirstOrDefault(tl => tl.Id == preferredTeamLeadId);
                    if (preferredTeamLead == null) continue;

                    var teamLeadWishlist = teamLeadsWishlists.FirstOrDefault(wl => wl.EmployeeId == preferredTeamLead.Id);
                    if (teamLeadWishlist != null && teamLeadWishlist.DesiredEmployees.Contains(junior.Id))
                    {
                        int j_index = Array.IndexOf(juniorWishlist.DesiredEmployees.ToArray(), preferredTeamLead.Id);
                        int juniorHappiness = j_index != -1 ? juniors.Count() - j_index : 0;

                        int t_index = Array.IndexOf(teamLeadWishlist.DesiredEmployees.ToArray(), junior.Id);
                        int teamLeadHappiness = t_index != -1 ? teamLeads.Count() - t_index : 0;

                        double happiness = CalculateHarmonic(juniorHappiness, teamLeadHappiness);

                        allPossiblePairs.Add((junior, preferredTeamLead, happiness));
                    }
                }
            }

            var sortedPairs = allPossiblePairs.OrderByDescending(p => p.happiness).ToList();

            foreach (var pair in sortedPairs)
            {
                if (!usedJuniors.Contains(pair.junior.Id) && !usedTeamLeads.Contains(pair.teamLead.Id))
                {
                    pairs.Add(new Team(pair.teamLead, pair.junior));
                    usedJuniors.Add(pair.junior.Id);
                    usedTeamLeads.Add(pair.teamLead.Id);
                }
            }

            return pairs;
        }

        private double CalculateHarmonic(int juniorHappiness, int teamLeadHappiness)
        {
            return 2.0 * juniorHappiness * teamLeadHappiness / (juniorHappiness + teamLeadHappiness);
        }

    }
}
