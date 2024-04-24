using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Transactions;
using TMC.Models;
using TMC_BAL.Interface;
using TMC_BAL.Repository;
using TMC_DAL.Models;

namespace TMC.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskrepository;
        private readonly ICategoryRepository _categoryRepository;

        public TaskController(ITaskRepository taskrepository, ICategoryRepository categoryRepository)
        {
            _taskrepository = taskrepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            TaskViewModel view = new TaskViewModel();
            view.AllTask = _taskrepository.GetAll();
            view.AllTask.Sort((a, b) => a.Id - b.Id);
            ViewBag.category = _categoryRepository.GetAll();
            return View(view);
        }


        public IActionResult CreateTask(TMC_DAL.Models.Task taskData)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    var newTask = new TMC_DAL.Models.Task
                    {
                        Taskname = taskData.Taskname,
                        Duedate = taskData.Duedate,
                        Assignee = taskData.Assignee,
                        Discription = taskData.Discription,
                        City = taskData.City
                    };

                    Category category = _categoryRepository.GetAll().FirstOrDefault(c => c.Name.Equals(taskData.Category, StringComparison.OrdinalIgnoreCase));

                    if (category == null)
                    {
                        category = new Category { Name = taskData.Category };
                        _categoryRepository.Add(category);
                    }

                    newTask.Categoryid = category.Id;
                    newTask.Category = category.Name;

                    _taskrepository.Add(newTask);
                    transaction.Complete();

                    TempData["Success"] = "Task Created Successfully";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                TempData["Error"] = "Error while Creating Task";
                return RedirectToAction("Index");
            }
        }

        public IActionResult EditTask(TMC_DAL.Models.Task taskData)
        {
            if (taskData.Id <= 0)
            {
                return BadRequest("Invalid Task ID");
            }

            try
            {
                TMC_DAL.Models.Task task = _taskrepository.Get(taskData.Id);
                if (task != null)
                {
                    task.Taskname = taskData.Taskname;
                    task.Duedate = taskData.Duedate;
                    task.Assignee = taskData.Assignee;

                    if (!string.Equals(taskData.Category, task.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        Category category = _categoryRepository.GetAll().FirstOrDefault(c => c.Name.Equals(taskData.Category, StringComparison.OrdinalIgnoreCase));

                        if (category == null)
                        {
                            category = new Category { Name = taskData.Category };
                            _categoryRepository.Add(category);
                        }

                        task.Categoryid = category.Id;
                        task.Category = category.Name;
                    }

                    task.City = taskData.City;
                    task.Discription = taskData.Discription;
                    _taskrepository.Update(task);
                    TempData["Success"] = "Task Edited Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Error while editing Task";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error while editing Task";
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public IActionResult GetTask(int id)
        {
            TMC_DAL.Models.Task task = _taskrepository.Get(id);
            if (task != null)
            {
                ViewBag.category = _categoryRepository.GetAll();
                Object data = new
                {
                    id = task.Id,
                    taskName = task.Taskname,
                    assignee = task.Assignee,
                    discription = task.Discription,
                    duedate = task.Duedate,
                    city = task.City,
                    category = task.Category
                };
                return Ok(data);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult DeleteTask(int id)
        {
            TMC_DAL.Models.Task task = _taskrepository.Get(id);
            if (task != null)
            {
                _taskrepository.Delete(task.Id);
            }
                return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}