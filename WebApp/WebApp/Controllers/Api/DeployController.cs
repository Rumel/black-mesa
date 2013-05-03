﻿using AttributeRouting;
using AttributeRouting.Web.Mvc;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers.Api
{
    [RoutePrefix("deploys")]
    public class DeployController : BaseController
    {
        private const int MaxReturnSize = 100;

        [GET("/")]
        public ActionResult Get()
        {
            var repository = new DeployRepository();
            return JsonNet(new Deploys
            {
                Items = repository.GetSince(DateTime.UtcNow.AddMonths(-2), MaxReturnSize).ToArray()
            });
        }

        [GET("all")]
        public ActionResult GetAll()
        {
            var repository = new DeployRepository();
            return JsonNet(new Deploys
            {
                Items = repository.GetSince(DateTime.UtcNow.AddYears(-1000), Int32.MaxValue).ToArray()
            });
        }

        [POST("/")]
        public ActionResult Post(NewDeploy deploy)
        {
            ConventNewDeployToDeploy(deploy);
            var repository = new DeployRepository();
            repository.Update(deploy);
            return JsonNet(new { success = true });
        }

        [POST("new")]
        public ActionResult NewDeploy(NewDeploy deploy)
        {
            ConventNewDeployToDeploy(deploy);
            deploy.DeployTime = DateTime.Now;
            var repository = new DeployRepository();
            repository.Add(deploy);
            return JsonNet(new { success = true });
        }

        private void ConventNewDeployToDeploy(NewDeploy deploy)
        {
            deploy.People = new People
            {
                CodeReviewers = deploy.CodeReview != null ? deploy.CodeReview.Split(',') : new string[0],
                Designers = deploy.Design != null ? deploy.Design.Split(',') : new string[0],
                Developers = deploy.Dev != null ? deploy.Dev.Split(',') : new string[0],
                ProjectManagers = deploy.ProjectManager != null ? deploy.ProjectManager.Split(',') : new string[0],
                Quails = deploy.Qa != null ? deploy.Qa.Split(',') : new string[0],
            };
        }
    }
}