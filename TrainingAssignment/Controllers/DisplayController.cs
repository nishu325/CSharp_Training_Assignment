using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Net;
using System.Data.Entity;
using NLog;
using System.Web.UI;
using TrainingAssignment.Auth;

namespace TrainingAssignment.Controllers
{
    /*
     * This controller is used for Adding new products,
     * Edit excisting products,
     * Delete any perticular or multiple products,
     * Search any product by it's name or category,
     * Display list of products available in database. 
     */
    [CustomAuthenticationFilter]
    [CustomAuthorize]
    public class DisplayController : Controller
    {
        public readonly Logger log = NLog.LogManager.GetCurrentClassLogger();

        /*Get method of Home page view*/
        [Route("Display/HomePage")]
        [HttpGet]
        
        public ActionResult Home()
        {
            return View();
            

        }

        /*Get method of AddProduct view*/
        [HttpGet]
        [Route("Display/AddNewProduct")]
        public ActionResult AddProduct()
        {

             return View();
           
        }

        /*Generating some random name for images to avoid same image name conflicts*/
        
        private Random random = new Random();
        [NonAction]
        public string RandomString(int length)
        {
            //log.Info("Random String method of \"Dashboard\" controller called");
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /*Post method of AddProduct view*/
        [HttpPost]
       
        [ValidateAntiForgeryToken]
        [Route("Display/AddNewProduct")]
        public ActionResult AddProduct(HttpPostedFileBase smallImage, HttpPostedFileBase largeImage, Products products)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (ProductManagementAssignmentEntities db = new ProductManagementAssignmentEntities())
                    {
                        Random r = new Random();
                        /*Check small image is selected or not*/
                        if (smallImage != null && smallImage.ContentLength > 0)
                        {



                            /*Get extension of image and check with image extension and store in targeted folder*/
                            string extension1 = Path.GetExtension(smallImage.FileName);
                            if (extension1.ToLower() == ".png" || extension1.ToLower() == ".jpg" || extension1.ToLower() == ".jpeg")
                            {
                                try
                                {
                                    string filename = "_" + RandomString(10) + Path.GetFileName(smallImage.FileName);
                                    string path = Path.Combine(Server.MapPath("~/Images"), filename);
                                    smallImage.SaveAs(path);
                                    products.SmallImage = "/Images/" + filename;
                                    log.Info("Small image stored with its ImagePath.");
                                }

                                catch (Exception ex)
                                {
                                    log.Error(ex, "Error occured during small image saving into database.");
                                    TempData["message"] = "Error - " + ex.Message;
                                    return View();
                                }

                            }

                            else
                            {

                                ViewBag.smallImage = "Please, Select Valid Image !";
                                return View();
                            }

                        } //End of if statement


                        else
                        {

                            ViewBag.smallImage_error = "You have not specified a Small Image.";
                            return View();
                        }

                        /*Check small image is selected or not*/
                        if (largeImage != null && largeImage.ContentLength > 0)
                        {


                            /*Get extension of image and check with image extension and store in targeted folder*/
                            string extension2 = Path.GetExtension(largeImage.FileName);
                            if (extension2.ToLower() == ".png" || extension2.ToLower() == ".jpg" || extension2.ToLower() == ".jpeg")
                            {
                                try
                                {

                                    string filename = "_" + RandomString(10) + Path.GetFileName(largeImage.FileName);
                                    string path = Path.Combine(Server.MapPath("~/Images"), filename);
                                    largeImage.SaveAs(path);
                                    products.LargeImage = "/Images/" + filename;
                                    log.Info("Large image stored with its ImagePath.");
                                }

                                catch (Exception ex)
                                {
                                    log.Error(ex, "Error occured during large image saving into database.");
                                    TempData["message"] = "Error - " + ex.Message;
                                    return View();
                                }

                            }

                            else
                            {

                                ViewBag.largeImage = "Please, Select Valid Image !";
                                return View();
                            }

                        } //End of if statement


                        else
                        {

                            ViewBag.largeImage_error = "You have not specified a Small Image.";
                            return View();
                        }

                        products.User_Id = (int)Session["Id"];
                        db.Products.Add(products);
                        db.SaveChanges();
                        TempData["message"] = products.Name + " Successfully Added ! ";
                        log.Info("New product added by user.");

                        return RedirectToAction("AddProduct", "Display");

                    } //End of Using statement

                } //End of try block

                catch (Exception ex)
                {
                    log.Error(ex, "Error occured while adding new product.");
                    TempData["message"] = "Error - " + ex.Message;
                    return View();
                }

            } //End of if statement
            return View();
        }


        /*Get method of ProductList view*/
        [HttpGet]
        [Route("Display/ProductListPage")]
       
        public ActionResult ProductList(int page = 1, string sort = "Name", string sortdir = "asc",string search="")
        {
            int pageSize = 3;
            int totalRecords = 0;
            if (pageSize < 1) page = 1;
            int skip = (page * pageSize) - pageSize;
            var data = GetProducts(search, sort, sortdir, skip, pageSize, totalRecords);
            ViewBag.TotalRows = totalRecords;
            log.Info("Display all product with details.");
            return View(data);
        }

        /*This mthod is used for display list of products that is added by perticular user by verifying it's user-id.*/
        public List<Products> GetProducts( string search,string sort, string sortdir, int skip, int pageSize,  int totalRecords)
        {
            int id = (int)Session["Id"];
            using (ProductManagementAssignmentEntities db = new ProductManagementAssignmentEntities())
            {


                var v = (from a in db.Products
                         where
                              a.User_Id.Equals(id) && (a.Name.Contains(search) || a.Category.Contains(search))
                         select a);
                totalRecords = v.Count();
                v = v.OrderBy(sort + " " + sortdir);
                if (pageSize > 0)
                {
                    v = v.Skip(skip).Take(pageSize);
                }
                return v.ToList();


            }
        }

   
        /*Get method of Delete any product*/
        [HttpGet]
        
        public ActionResult Delete(int? id)
        {
            using (ProductManagementAssignmentEntities db = new ProductManagementAssignmentEntities())
            {
                try
                {
                    Products product = db.Products.Find(id);

                    string smallImage_fileName = product.SmallImage;
                    string smallImage_path = Request.MapPath("~" + smallImage_fileName);

                    /*Check small image is available or not*/
                    if (System.IO.File.Exists(smallImage_path))
                    {
                        System.IO.File.Delete(smallImage_path);

                    }

                    string largeImage_fileName = product.LargeImage;
                    string largeImage_path = Request.MapPath("~" + largeImage_fileName);

                    /*Check large image is available or not*/
                    if (System.IO.File.Exists(largeImage_path))
                    {
                        System.IO.File.Delete(largeImage_path);

                    }

                    //Delete whole record of that perticular id in database table
                    db.Products.Remove(product);
                    db.SaveChanges();
                    TempData["message"] = product.Name + "Deleted successfully";
                    log.Info("The product is deleted with from database.");

                    return RedirectToAction("ProductList", "Display");

                } //End of try block.

                catch (Exception ex)
                {
                    log.Error(ex, "Error while deleting data.");
                    TempData["message"] = "Error - " + ex.Message;
                    return View();

                }
            } //End of using statement.

        }


        /*For multiple delete (Delete selected item)*/
        public ActionResult Selected_delete(String[] checkbox)
        {
            try
            {
                using (ProductManagementAssignmentEntities db = new ProductManagementAssignmentEntities())
                {
                    int[] getId = null;
                    if (checkbox != null)
                    {
                        getId = new int[checkbox.Length];
                        int j = 0;


                        foreach (string i in checkbox)
                        {
                            int.TryParse(i, out getId[j++]);
                        }


                        List<Products> getproductids = new List<Products>();

                        getproductids = db.Products.Where(x => getId.Contains(x.Id)).ToList();

                        /*Remove seleted items*/
                        foreach (var s in getproductids)
                        {
                            db.Products.Remove(s);
                        }
                        db.SaveChanges();
                        TempData["message"] = checkbox.Length + " Products Successfully Removed ";
                        log.Info("All selected items are now deleted.");
                        return RedirectToAction("ProductList", "Display");
                    }//End of if statement

                }//End of usng statement.

            }

            catch (Exception ex)
            {
                log.Error(ex, "Error during delet multiple items from database.");
                TempData["message"] = "Error - " + ex.Message;
                return View();
            }
            return View();

        }

        /*Get method of Edit any product*/
        [HttpGet]
        [Route("Display/EditProduct")]
        public ActionResult Edit(int? id)
        {
            using (ProductManagementAssignmentEntities db = new ProductManagementAssignmentEntities())
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Products product = db.Products.Find(id);
                Session["smallImage"] = product.SmallImage;
                Session["largeImage"] = product.LargeImage;
                if (product == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id = id;
                log.Info("Get all the details of selected product for Edit their details inside (Get) method.");
                return View(product);
            } //End of using statement


        }

        /*Edit any product POST Method*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Display/EditProduct")]
        public ActionResult Edit(HttpPostedFileBase smallImage, HttpPostedFileBase largeImage, Products product)
        {
            // log.Info("Calling Edit (Post)method of \"Dahsboard\" controller");
            using (ProductManagementAssignmentEntities db = new ProductManagementAssignmentEntities())
            {
                if (ModelState.IsValid)
                {
                    Random random = new Random();
                    /*This block will execute while both images are changed from previous images.*/
                    if ((smallImage != null && smallImage.ContentLength > 0) && (largeImage != null && largeImage.ContentLength > 0))
                    {

                        try
                        {
                            string filename = "_" + RandomString(10) + Path.GetFileName(smallImage.FileName);
                            string path = Path.Combine(Server.MapPath("~/Images"), filename);
                            smallImage.SaveAs(path);
                            product.SmallImage = "/Images/" + filename;

                            string filename1 = "_" + RandomString(10) + Path.GetFileName(largeImage.FileName);
                            string path1 = Path.Combine(Server.MapPath("~/Images"), filename1);
                            largeImage.SaveAs(path1);
                            product.LargeImage = "/Images/" + filename1;


                        }

                        catch (Exception ex)
                        {
                            log.Error(ex, "Error occured while update small and large images.");
                            TempData["message"] = "ERROR:" + ex.Message;
                            return View();
                        }
                        product.User_Id = (int)Session["Id"];
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["message"] = product.Name + " successfully Updated";
                        log.Info("Details of product updated into database.");

                        return RedirectToAction("Edit", "Display");

                    } //End of inner if block


                    /*This block will execute while only small image is changed and large image will remain same.*/
                    else if ((smallImage != null && smallImage.ContentLength > 0) && (largeImage == null))
                    {

                        try
                        {

                            string filename = "_" + RandomString(10) + Path.GetFileName(smallImage.FileName);
                            string path = Path.Combine(Server.MapPath("~/Images"), filename);
                            smallImage.SaveAs(path);
                            product.SmallImage = "/Images/" + filename;
                            product.LargeImage = Session["largeImage"].ToString();

                        }

                        catch (Exception ex)
                        {
                            log.Error(ex, "Error occured while update small image.");
                            TempData["message"] = "ERROR:" + ex.Message;
                            return View();
                        }
                        product.User_Id = (int)Session["Id"];
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["message"] = product.Name + " successfully Updated";
                        log.Info("Details of product updated into database.");

                        return RedirectToAction("Edit", "Display");

                    } //End of else if block


                    /*This block will execute while small image will remain same and large image is changed.*/
                    else if ((smallImage == null) && (largeImage != null && largeImage.ContentLength > 0))
                    {

                        try
                        {

                            string filename1 = "_" + RandomString(10) + Path.GetFileName(largeImage.FileName);
                            string path1 = Path.Combine(Server.MapPath("~/Images"), filename1);
                            largeImage.SaveAs(path1);
                            product.LargeImage = "/Images/" + filename1;
                            product.SmallImage = Session["smallImage"].ToString();

                        }

                        catch (Exception ex)
                        {
                            log.Error(ex, "Error occured while update large image.");
                            TempData["message"] = "ERROR:" + ex.Message;
                            return View();
                        }
                        product.User_Id = (int)Session["Id"];
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["message"] = product.Name + " successfully Updated";
                        log.Info("Details of product updated into database.");

                        return RedirectToAction("Edit", "Display");

                    } //End of else if block

                    /*Everything is valid and update into database*/
                    else
                    {
                        product.User_Id = (int)Session["Id"];
                        product.SmallImage = Session["smallImage"].ToString();
                        product.LargeImage = Session["largeImage"].ToString();
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["message"] = product.Name + " successfully Updated";
                        log.Info("Details of product updated into database.");

                        return RedirectToAction("Edit", "Display");

                    } //End of else block

                } //End of outer if block

            } //End of using statement

            return View();
        }



        /*For back to home page while user is on add new product page.*/
        [Route("Display/HomePage")]
        public ActionResult Cancle()
        {
            return RedirectToAction("Home", "Display");
        }

    }//End of controller 

} //End of namespace