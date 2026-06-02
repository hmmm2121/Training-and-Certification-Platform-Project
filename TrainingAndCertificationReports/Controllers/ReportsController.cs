using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace TrainingAndCertificationReports.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApiAuthService _authService;

        public ReportsController(ApiAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> EnrollmentStats()
        {
            // If no token in session, send user to login page
            if (!_authService.HasToken())
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _authService.GetAuthorizedClient();

            List<EnrollmentStatViewModel>? stats = null;

            try
            {
                stats = await client.GetFromJsonAsync<List<EnrollmentStatViewModel>>(
                    "api/reports/enrollment-stats");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            stats ??= new List<EnrollmentStatViewModel>();
            return View(stats);
        }

        public async Task<IActionResult> InstructorWorkload()
        {
            if (!_authService.HasToken())
                return RedirectToAction("Login", "Account");

            var client = _authService.GetAuthorizedClient();

            List<InstructorWorkloadViewModel>? data = null;
            try
            {
                data = await client.GetFromJsonAsync<List<InstructorWorkloadViewModel>>(
                    "api/reports/instructor-workload");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            data ??= new List<InstructorWorkloadViewModel>();
            return View(data);
        }
        public async Task<IActionResult> Revenue()
        {
            if (!_authService.HasToken())
                return RedirectToAction("Login", "Account");

            var client = _authService.GetAuthorizedClient();

            RevenueReportViewModel? report = null;

            try
            {
                report = await client.GetFromJsonAsync<RevenueReportViewModel>(
                    "api/reports/revenue");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            report ??= new RevenueReportViewModel();
            return View(report);
        }
        public async Task<IActionResult> CertificationRates()
        {
            if (!_authService.HasToken())
                return RedirectToAction("Login", "Account");

            var client = _authService.GetAuthorizedClient();

            List<CertificationRateViewModel>? data = null;
            try
            {
                data = await client.GetFromJsonAsync<List<CertificationRateViewModel>>(
                    "api/reports/certification-rates");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            data ??= new List<CertificationRateViewModel>();
            return View(data);
        }
        public async Task<IActionResult> CoursePopularity()
        {
            if (!_authService.HasToken())
                return RedirectToAction("Login", "Account");

            var client = _authService.GetAuthorizedClient();

            List<CoursePopularityViewModel>? data = null;
            try
            {
                data = await client.GetFromJsonAsync<List<CoursePopularityViewModel>>(
                    "api/reports/course-popularity");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            data ??= new List<CoursePopularityViewModel>();
            return View(data);
        }
        public async Task<IActionResult> RoomUtilization()
        {
            if (!_authService.HasToken())
                return RedirectToAction("Login", "Account");

            var client = _authService.GetAuthorizedClient();

            List<RoomUtilizationViewModel>? data = null;
            try
            {
                data = await client.GetFromJsonAsync<List<RoomUtilizationViewModel>>(
                    "api/reports/room-utilization");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            data ??= new List<RoomUtilizationViewModel>();
            return View(data);
        }
        public async Task<IActionResult> TraineePayments()
        {
            if (!_authService.HasToken())
                return RedirectToAction("Login", "Account");

            var client = _authService.GetAuthorizedClient();

            List<TraineePaymentViewModel>? data = null;
            try
            {
                data = await client.GetFromJsonAsync<List<TraineePaymentViewModel>>(
                    "api/reports/trainee-payments");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            data ??= new List<TraineePaymentViewModel>();
            return View(data);
        }
    }
}