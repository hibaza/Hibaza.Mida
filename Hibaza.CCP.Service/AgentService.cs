using Hangfire;
using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public class AgentService : IAgentService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IFirebaseAgentRepository _fbAgentRepository;
        public AgentService(IAgentRepository agentRepository, IFirebaseAgentRepository fbAgentRepository)
        {
            _agentRepository = agentRepository;
            _fbAgentRepository = fbAgentRepository;
        }


        public string ToogleRole(string business_id, string id, string role)
        {
            Agent agent = GetById(business_id, id);
            if (agent != null && !string.IsNullOrWhiteSpace(business_id) && !string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(role))
            {

                if (role == "admin")
                {
                    if (agent.role == "admin") { agent.role = "agent"; }
                    else
                    {
                        agent.role = "admin";
                    }
                    Create(agent); //TODO: temporarily use, shoudld be removed
                }

                //var list = _agentRepository.GetRoles(business_id, id);
                //if (list != null && list.Contains(role))
                //{
                //    _agentRepository.DeleteRole(business_id, id, role);
                //}
                //else
                //{
                //    _agentRepository.AddRole(business_id, id, role);
                //}

            }
            return id;
        }

        public string CopyToReatimeDB(string business_id, string id, DateTime time)
        {
            var agent = GetById(business_id, id);

            CopyToReatimeDB(business_id, agent);

            return agent.id;
        }

        private string CopyToReatimeDB(string business_id, Agent agent)
        {
            try
            {
                if (agent != null) _fbAgentRepository.Upsert(business_id, new AgentModel(agent));
            }
            catch (Exception ex) { }
            return agent.id;
        }

        public string Create(Agent data)
        {
            data.updated_time = DateTime.UtcNow;
            _agentRepository.Upsert(data);
            try
            {
                CopyToReatimeDB(data.business_id, data);
            }
            catch
            {
                BackgroundJob.Enqueue<AgentService>(x => x.CopyToReatimeDB(data.business_id, data.id, (DateTime)data.updated_time));
            }
            return data.id;
        }


        public async Task<IEnumerable<Agent>> GetAgents(string business_id, int pageIndex, int pageSize)
        {
            return await _agentRepository.GetAgents(business_id, new Domain.Models.Paging { Limit = pageSize });

        }

        public async Task<IEnumerable<Agent>> GetOnlineAgents(string business_id, int pageIndex, int pageSize)
        {
            return await _agentRepository.GetOnlineAgents(business_id, new Domain.Models.Paging { Limit = pageSize });

        }

        public Agent SetWorkStatus(string id, string status, DateTime time)
        {
            if (string.IsNullOrEmpty(id)) return null;
            var agent = GetById(id);
            if (agent != null && (status == "online" || status == "busy"))
            {
                agent.active = status == "online";
                agent.status = agent.login_status == "online" ? status : agent.login_status;
                if (status == "online") { agent.last_acted_time = time; }
                Create(agent);
            }

            return agent;
        }

        public Agent SetLoginStatus(Agent agent, string status, DateTime time)
        {
            if (agent != null && (status == "online" || status == "locked" || status == "offline"))
            {
                agent.status = status == "online" ? agent.active ? "online" : "busy" : status;
                agent.login_status = status;
                if (status == "online") { agent.last_acted_time = time; agent.last_loggedin_time = time; }
                else {
                    if (status == "offline")
                    {
                        agent.last_loggedout_time = DateTime.UtcNow;

                    }
                }
                Create(agent);
            }
            return agent;
        }

        public async System.Threading.Tasks.Task SetLastActivityTime(string id, long timestamp)
        {
            if (string.IsNullOrEmpty(id)) return;
            DateTime time = Core.Helpers.CommonHelper.UnixTimestampToDateTime(timestamp).ToUniversalTime();
            var agent = GetById(id);
            //if (agent != null && agent.last_acted_time <= time)
            //{
            //    agent.last_acted_time = time;
            //    Create(agent);
            //}
            if (agent != null)
            {
                agent.last_acted_time = time;
                Create(agent);
            }
        }

        public async Task<int> SetBusyAllInActivityAgents(string business_id, int minutes)
        {
            int count = 0;
            foreach (var agent in await GetAgents(business_id, 0, 100))
            {
                if (agent.status == "online" && ((DateTime.UtcNow - (agent.last_acted_time ?? DateTime.MinValue)).TotalMinutes > minutes))
                {
                    count++;
                    SetWorkStatus(agent.id, "busy", DateTime.UtcNow);
                }
            }
            return count;
        }

        public async Task<int> LogoutAllInActivityAgents(string business_id, int minutes)
        {
            int count = 0;
            foreach (var agent in await GetAgents(business_id, 0, 100))
            {
                //if (agent.login_status != "offline" && ((DateTime.UtcNow - (agent.last_acted_time ?? DateTime.MinValue)).TotalMinutes > minutes))
                var totalMinutes = (DateTime.UtcNow - (agent.last_acted_time ?? DateTime.MinValue)).TotalMinutes;
                if (totalMinutes > minutes && totalMinutes > 24 * 60)
                {
                    count++;
                    SetLoginStatus(agent.id, "offline", DateTime.UtcNow);
                }
            }
            return count;
        }

        public Agent SetLoginStatus(string id, string status, DateTime time)
        {            
            if (string.IsNullOrEmpty(id)) return null;
            var agent = _agentRepository.GetById(id);

            return SetLoginStatus(agent, status, time);
        }

        public Agent GetById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return _agentRepository.GetById(id);
            }
            return null;
        }

        public Agent GetById(string business_id, string id)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(business_id))
            {
                return _agentRepository.GetById(business_id, id);
            }
            return null;
        }

        public Agent GetSingleOrDefaultByUserName(string userName)
        {
            var rs = _agentRepository.GetByUserName(userName);
            return rs.Count() == 1 ? rs.First() : null;
        }


        public bool Delete(string business_id, string id)
        {
            if (!string.IsNullOrWhiteSpace(business_id) && !string.IsNullOrWhiteSpace(id))
            {
                //_agentRepository.DeleteRoleAll(business_id, id);
                Agent agent = _agentRepository.GetById(id);
                agent.role = null;
                _agentRepository.Upsert(agent);
                //_agentRepository.Delete(business_id, id);
                _fbAgentRepository.Delete(business_id, id);
                return true;
            }
            else return false;
        }


    }
}
