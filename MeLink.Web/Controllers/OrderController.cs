// Controllers/OrderController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeLink.Web.Data;
using MeLink.Web.Models;
using MeLink.Web.ViewModels;

namespace MeLink.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // صفحة البحث عن الأدوية والصيدليات
        public async Task<IActionResult> Index(string? searchTerm, string? supplierId)
        {
           

                var currentUser = await _userManager.GetUserAsync(User);
            // التحقق من أن المستخدم مريض وتخزين النتيجة في ViewBag
            ViewBag.IsPatient = currentUser is Patient;
            // تأكد إن المستخدم مريض
            if (currentUser is not Patient)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var query = _context.Inventories
                .Include(i => i.Medicine)
                .Include(i => i.User)
                .Where(i => i.User is Pharmacy && i.IsAvailable && i.StockQuantity > 0);
            if (!string.IsNullOrEmpty(supplierId))
            {
                query = query.Where(i => i.UserId == supplierId);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(i =>
                    i.Medicine!.GenericName.Contains(searchTerm) ||
                    i.Medicine!.BrandName!.Contains(searchTerm) ||
                    i.Medicine!.ActiveIngredient!.Contains(searchTerm));
            }
            var availableMedicines = await query.ToListAsync();

            var viewModelList = availableMedicines.Select(i =>
            {
                var distance = CalculateDistance(
                    currentUser.Latitude.Value,
                    currentUser.Longitude.Value,
                    i.User!.Latitude.Value,
                    i.User!.Longitude.Value
                );

                return new MedicineInventoryViewModel
                {
                    MedicineId = i.MedicineId,
                    GenericName = i.Medicine!.GenericName,
                    BrandName = i.Medicine.BrandName,
                    DosageForm = i.Medicine.DosageForm,
                    Strength = i.Medicine.Strength,
                    PharmacyId = i.UserId,
                    PharmacyName = i.User!.DisplayName!,
                    PharmacyAddress = i.User.Address,
                    Price = i.Price,
                    StockQuantity = i.StockQuantity,
                    InventoryId = i.Id,
                    DistanceInKm = distance
                };
            })
                .OrderBy(m => m.DistanceInKm) // الترتيب حسب المسافة
                 .ToList();
          

            var viewModel = new OrderSearchViewModel
            {
                SearchTerm = searchTerm,
                AvailableMedicines = viewModelList,
                SupplierId = supplierId
            };

            return View(viewModel);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrescriptionOrder(CreatePrescriptionOrderViewModel model)
        {
            // Repopulate SupplierName if model state is invalid
            if (!ModelState.IsValid)
            {
                var supplier = await _context.Users.FindAsync(model.SupplierId);
                model.SupplierName = supplier?.DisplayName;
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // 1. Create and save the order first to get an ID
            var order = new Order
            {
                FromUserId = currentUser.Id,
                ToUserId = model.SupplierId,
                Status = OrderStatus.Pending,
                OrderType = "Patient-Pharmacy-PrescriptionOnly"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Order gets an ID here

            // 2. Save the prescription file, linking it to the new order's ID
            if (model.PrescriptionFile != null)
            {
                var prescription = await SavePrescriptionAsync(model.PrescriptionFile, order.Id, currentUser.Id);
                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync(); // Save the prescription
            }

            TempData["Success"] = "Your prescription has been sent successfully!";
            return RedirectToAction("MyOrders");
        }

        [HttpGet]
        public async Task<IActionResult> CreatePrescriptionOrder(string supplierId)
        {
            var supplier = await _context.Users.FindAsync(supplierId);
            if (supplier == null)
            {
                return NotFound();
            }

            var model = new CreatePrescriptionOrderViewModel
            {
                SupplierId = supplier.Id,
                SupplierName = supplier.DisplayName
            };

            return View(model);
        }
        public async Task<IActionResult> NearbyPharmacies()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser is not Patient || currentUser.Latitude == null || currentUser.Longitude == null)
            {
                // For demo purposes, we can use fixed coordinates if user's location is missing
                currentUser.Latitude = 30.0444;
                currentUser.Longitude = 31.2357;
            }

            // Get all Pharmacy users
            var pharmacies = await _context.Users.OfType<Pharmacy>().ToListAsync();

            var viewModel = new NearbyPharmaciesViewModel();

            foreach (var pharmacy in pharmacies)
            {
                // Calculate distance only if pharmacy has location data
                double? distance = null;
                if (pharmacy.Latitude != null && pharmacy.Longitude != null)
                {
                    distance = CalculateDistance(
                        currentUser.Latitude.Value,
                        currentUser.Longitude.Value,
                        pharmacy.Latitude.Value,
                        pharmacy.Longitude.Value
                    );
                }

                viewModel.Pharmacies.Add(new PharmacyViewModel
                {
                    UserId = pharmacy.Id,
                    DisplayName = pharmacy.DisplayName!,
                    Address = pharmacy.Address,
                    City = pharmacy.City,
                    DistanceInKm = distance
                });
            }

            // Sort by distance (closest first)
            viewModel.Pharmacies = viewModel.Pharmacies.OrderBy(p => p.DistanceInKm).ToList();

            return View(viewModel);
        }
        // صفحة إنشاء طلب جديد
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var viewModel = new CreateOrderViewModel
            {
                FromUserId = currentUser!.Id,
                FromUserName = currentUser.DisplayName!,
                IsPatient = currentUser is Patient
            };

            return View(viewModel);
        }

        // إضافة دواء للطلب
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser is not Patient)
            {
                return Json(new { success = false, message = "غير مسموح" });
            }

            var inventory = await _context.Inventories
                .Include(i => i.Medicine)
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == request.InventoryId);

            if (inventory == null || !inventory.IsAvailable || inventory.StockQuantity < request.Quantity)
            {
                return Json(new { success = false, message = "الدواء غير متوفر بالكمية المطلوبة" });
            }

            

            return Json(new
            {
                success = true,
                item = new
                {
                    inventoryId = inventory.Id,
                    medicineName = inventory.Medicine!.GenericName,
                    brandName = inventory.Medicine.BrandName,
                    pharmacyName = inventory.User!.DisplayName,
                    price = inventory.Price,
                    quantity = request.Quantity,
                    total = inventory.Price * request.Quantity
                }
            });
        }
        // تأكيد الطلب
        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(CreateOrderViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);


            // تعبئة بيانات المريض إذا كانت فارغة
            if (string.IsNullOrEmpty(model.FromUserId))
            {
                model.FromUserId = currentUser.Id;
            }

            if (model.OrderItems == null || !model.OrderItems.Any())
            {
                TempData["Error"] = "لا توجد أدوية في الطلب";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.PharmacyId))
            {
                TempData["Error"] = "لم يتم تحديد الصيدلية";
                return RedirectToAction("Index");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // إنشاء الطلب الرئيسي
                var order = new Order
                {
                    FromUserId = currentUser.Id,
                    ToUserId = model.PharmacyId,
                    Status = OrderStatus.Pending,
                    OrderType = "Patient-Pharmacy"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // إضافة تفاصيل الطلب
                foreach (var item in model.OrderItems)
                {
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.Id == item.InventoryId);

                    if (inventory == null || inventory.StockQuantity < item.Quantity)
                    {
                        throw new Exception($"الدواء '{item.MedicineName}' غير متوفر بالكمية المطلوبة");
                    }

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        MedicineId = inventory.MedicineId,
                        Quantity = item.Quantity
                    };

                    _context.OrderDetails.Add(orderDetail);

                    // تقليل المخزون
                    inventory.StockQuantity -= item.Quantity;
                    if (inventory.StockQuantity <= 0)
                    {
                        inventory.IsAvailable = false;
                    }
                }

                // حفظ الروشتة إن وجدت
                if (model.PrescriptionFile != null)
                {
                    var prescription = await SavePrescriptionAsync(model.PrescriptionFile, order.Id, currentUser.Id);
                    _context.Prescriptions.Add(prescription);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "تم إنشاء طلبك بنجاح!";

                // توجيه لصفحة الفاتورة
                return RedirectToAction("Invoice", new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "حدث خطأ أثناء إنشاء الطلب: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // عرض الفاتورة
        public async Task<IActionResult> Invoice(int orderId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var order = await _context.Orders
                .Include(o => o.FromUser)
                .Include(o => o.ToUser)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Medicine)
                .FirstOrDefaultAsync(o => o.Id == orderId && (o.FromUserId == currentUser!.Id||o.ToUserId==currentUser!.Id));

            if (order == null)
            {
                return NotFound();
            }

            // Get all inventory items for this order's pharmacy at once
            var pharmacyInventory = await _context.Inventories
                .Where(i => i.UserId == order.ToUserId && order.Items.Select(item => item.MedicineId).Contains(i.MedicineId))
                .ToDictionaryAsync(i => i.MedicineId, i => i);
            // حساب التفاصيل المالية
            var invoiceItems = new List<InvoiceItemViewModel>();
            decimal totalAmount = 0;

            foreach (var item in order.Items)
            {
                if (pharmacyInventory.TryGetValue(item.MedicineId, out var inventory))
                {
                    var itemTotal = (inventory.Price) * item.Quantity;
                    totalAmount += itemTotal;

                    invoiceItems.Add(new InvoiceItemViewModel
                    {
                        MedicineName = item.Medicine!.GenericName,
                        BrandName = item.Medicine.BrandName,
                        Quantity = item.Quantity,
                        UnitPrice = inventory.Price,
                        Total = itemTotal
                    });
                }
            }

            var viewModel = new InvoiceViewModel
            {
                OrderId = order.Id,
                OrderDate = order.CreatedAt,
                PatientName = order.FromUser!.DisplayName!,
                PharmacyName = order.ToUser!.DisplayName!,
                PharmacyAddress = order.ToUser.Address,
                Items = invoiceItems,
                TotalAmount = totalAmount,
                Status = order.Status.ToString()
            };

            return View(viewModel);
        }

        // طلبات المريض
        public async Task<IActionResult> MyOrders()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var orders = await _context.Orders
                .Include(o => o.ToUser)
                .Where(o => o.FromUserId == currentUser!.Id)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderSummaryViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.CreatedAt,
                    PharmacyName = o.ToUser!.DisplayName!,
                    Status = o.Status.ToString(),
                    TotalItems = o.Items.Count()
                })
                .ToListAsync();

            return View(orders);
        }

        private async Task<Prescription> SavePrescriptionAsync(IFormFile file, int orderId, string userId)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "prescriptions");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new Prescription
            {
                OrderId = orderId,
                ImagePath = $"/prescriptions/{fileName}",
                UploadedByUserId = userId
            };
        }
        // API للبحث السريع في الأدوية
        [HttpGet]
        public async Task<IActionResult> SearchMedicines(string term, string? forUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Forbid();
            }

            var inventoriesQuery = _context.Inventories
                .Include(i => i.Medicine)
                .Include(i => i.User)
                .Where(i => i.IsAvailable && i.StockQuantity > 0);


            // Filter by search term
            if (!string.IsNullOrEmpty(term))
            {
                inventoriesQuery = inventoriesQuery.Where(i =>
                    i.Medicine!.GenericName.Contains(term) ||
                    (i.Medicine!.BrandName != null && i.Medicine.BrandName.Contains(term)) ||
                    (i.Medicine!.ActiveIngredient != null && i.Medicine.ActiveIngredient.Contains(term)));
            }

            // Determine allowed supplier IDs based on the current user's role
            var allowedSupplierIds = Enumerable.Empty<string>();

            if (currentUser is Patient)
            {
                // Patient can only order from Pharmacies
                allowedSupplierIds = _context.Users.OfType<Pharmacy>().Select(p => p.Id);
            }
            else if (currentUser is Pharmacy)
            {
                // Pharmacy can order from all Warehouses, Companies, and Manufacturers
                var warehouseIds = _context.Users.OfType<MedicineWarehouse>().Select(w => w.Id);
                var companyIds = _context.Users.OfType<DistributionCompany>().Select(c => c.Id);
                var manufacturerIds = _context.Users.OfType<Manufacturer>().Select(m => m.Id);
                allowedSupplierIds = warehouseIds.Concat(companyIds).Concat(manufacturerIds);
            }
            else if (currentUser is MedicineWarehouse)
            {
                // Warehouse can order from other Warehouses, Companies, and Manufacturers
                var otherWarehouseIds = _context.Users.OfType<MedicineWarehouse>().Where(w => w.Id != currentUser.Id).Select(w => w.Id);
                var companyIds = _context.Users.OfType<DistributionCompany>().Select(c => c.Id);
                var manufacturerIds = _context.Users.OfType<Manufacturer>().Select(m => m.Id);
                allowedSupplierIds = otherWarehouseIds.Concat(companyIds).Concat(manufacturerIds);
            }
            else if (currentUser is DistributionCompany)
            {
                // Company orders from Manufacturers
                allowedSupplierIds = _context.Users.OfType<Manufacturer>().Select(m => m.Id);
            }
            else
            {
                // Any other user type (e.g., Manufacturer) cannot search for medicines to order.
                return Json(new List<MedicineSearchResult>());
            }

            // Apply the user-specific filter to the main query
            inventoriesQuery = inventoriesQuery.Where(i => allowedSupplierIds.Contains(i.UserId));

            var results = inventoriesQuery
                .Select(i => new
                {
                    Inventory = i,
                    DiscountPercentage = (i.DiscountPrice.HasValue && i.Price > 0) ? ((i.Price - i.DiscountPrice.Value) / i.Price) * 100 : (decimal?)null
                });

            if (currentUser is not Patient)
            {
                results = results.OrderByDescending(r => r.DiscountPercentage);
            }

            var medicines = await results
                .Select(r => new MedicineSearchResult
                {
                    InventoryId = r.Inventory.Id,
                    MedicineName = r.Inventory.Medicine!.GenericName,
                    BrandName = r.Inventory.Medicine!.BrandName,
                    SourceName = r.Inventory.User!.DisplayName!,
                    SourceId = r.Inventory.User.Id,
                    Price = r.Inventory.Price,
                    Stock = r.Inventory.StockQuantity,
                    DiscountPercentage = r.DiscountPercentage
                })
                .Take(20)
                .ToListAsync();

            return Json(medicines);
        }
        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            // ملاحظة: تأكد من أن اسم الـ DbContext (هنا _context) يطابق الاسم في مشروعك
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction(nameof(MyOrders));
            }

            if (order.Status != OrderStatus.Pending)
            {
                TempData["ErrorMessage"] = "This order can no longer be cancelled.";
                return RedirectToAction(nameof(MyOrders));
            }

            order.Status = OrderStatus.Cancelled;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Order #{orderId} has been successfully cancelled.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while cancelling the order.";
            }

            return RedirectToAction(nameof(MyOrders));
        }

        public async Task<IActionResult> ApproveOrder(int orderId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction(nameof(IncomingOrders));
            }

            if (order.ToUserId != currentUser.Id)
            {
                return Forbid();
            }

            if (order.Status != OrderStatus.Pending)
            {
                TempData["ErrorMessage"] = "This order has already been processed.";
                return RedirectToAction(nameof(IncomingOrders));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                order.Status = OrderStatus.Approved;

                var requesterId = order.FromUserId;
                foreach (var item in order.Items)
                {
                    var requesterInventoryItem = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.UserId == requesterId && i.MedicineId == item.MedicineId);

                    if (requesterInventoryItem != null)
                    {
                        requesterInventoryItem.StockQuantity += item.Quantity;
                    }
                    else
                    {
                        var supplierInventoryItem = await _context.Inventories
                            .AsNoTracking()
                            .FirstOrDefaultAsync(i => i.UserId == order.ToUserId && i.MedicineId == item.MedicineId);

                        var newInventoryItem = new Inventory
                        {
                            UserId = requesterId,
                            MedicineId = item.MedicineId,
                            StockQuantity = item.Quantity,
                            Price = supplierInventoryItem?.Price ?? 0,
                            IsAvailable = true,
                            LastUpdated = DateTime.UtcNow
                        };
                        _context.Inventories.Add(newInventoryItem);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = $"Order #{orderId} has been successfully approved.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "An error occurred while approving the order: " + ex.Message;
            }

            return RedirectToAction(nameof(IncomingOrders));
        }

        public async Task<IActionResult> Reorder(int orderId)
        {
            // Logic to recreate order with same items
            return RedirectToAction("Create");
        }
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c; // Distance in km
            return distance;
        }
        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public async Task<IActionResult> BrowseMedicines(string? supplierId, string? searchTerm)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            ViewBag.IsPatient=false;

            var query = _context.Inventories
                  .Include(i => i.Medicine)
                  .Include(i => i.User).AsQueryable();

            if (!string.IsNullOrEmpty(supplierId))
            {
                query = query.Where(i => i.UserId == supplierId && i.IsAvailable && i.StockQuantity > 0);
            }
            else
            {
                if (currentUser is Pharmacy)
                {
                    query = query.Where(i => i.User is MedicineWarehouse || i.User is DistributionCompany || i.User is Manufacturer);
                }
                else if (currentUser is MedicineWarehouse)
                {
                    query = query.Where(i => (i.User is DistributionCompany || i.User is Manufacturer) || (i.User is MedicineWarehouse && i.UserId != currentUser.Id));
                }
                else if (currentUser is DistributionCompany)
                {
                    query = query.Where(i => i.User is Manufacturer);
                }
            }
           

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(i =>
                    i.Medicine!.GenericName.Contains(searchTerm) ||
                    i.Medicine!.BrandName!.Contains(searchTerm) ||
                    i.Medicine!.ActiveIngredient!.Contains(searchTerm));
            }

            var availableMedicines = await query
                .Select(i => new
                {
                    Inventory = i,
                    DiscountPercentage = (i.DiscountPrice.HasValue && i.Price > 0) ? ((i.Price - i.DiscountPrice.Value) / i.Price) * 100 : (decimal?)null
                })
                .OrderByDescending(r => r.DiscountPercentage)
                .Select(r => new MedicineInventoryViewModel
                {
                    MedicineId = r.Inventory.MedicineId,
                    GenericName = r.Inventory.Medicine!.GenericName,
                    BrandName = r.Inventory.Medicine.BrandName,
                    DosageForm = r.Inventory.Medicine.DosageForm,
                    Strength = r.Inventory.Medicine.Strength,
                    PharmacyId = r.Inventory.UserId,
                    PharmacyName = r.Inventory.User!.DisplayName!,
                    PharmacyAddress = r.Inventory.User.Address,
                    Price = r.Inventory.Price,
                    StockQuantity = r.Inventory.StockQuantity,
                    InventoryId = r.Inventory.Id,
                    DiscountPercentage = r.DiscountPercentage
                }).ToListAsync();

            var viewModel = new OrderSearchViewModel
            {
                SearchTerm = searchTerm,
                AvailableMedicines = availableMedicines,
                SupplierId = supplierId // **تعبئة الخاصية الجديدة هنا**
            };

            return View("Index", viewModel); // استخدم نفس الـ View الخاص بصفحة البحث
        }
        public async Task<IActionResult> IncomingOrders()
        {
            var currentUser = await _userManager.GetUserAsync(User);

      
            if (currentUser is Patient)
            {
                return Forbid();
            }

            var orders = await _context.Orders
                .Include(o => o.FromUser)
                .Include(o => o.Items)
                .Where(o => o.ToUserId == currentUser!.Id)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderSummaryViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.CreatedAt,
                    PharmacyName = o.FromUser!.DisplayName!,
                    Status = o.Status.ToString(),
                    TotalItems = o.Items.Count()
                })
                .ToListAsync();

            var viewModel = new IncomingOrdersViewModel
            {
                IncomingOrders = orders
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Indebtedness()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // 1. Get all orders from the current user to suppliers that allow installments.
            var orders = await _context.Orders
                .Include(o => o.ToUser)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Medicine)
                .Where(o => o.FromUserId == currentUser.Id && o.ToUser.Installment == true)
                .ToListAsync();

            // 2. Group orders by the supplier (ToUser).
            var groupedOrders = orders.GroupBy(o => o.ToUser);

            var indebtednessSummaries = new List<IndebtednessSummaryViewModel>();
            decimal totalIndebtedness = 0;

            // 3. Process each group to calculate indebtedness.
            foreach (var group in groupedOrders)
            {
                var company = group.Key;
                decimal companyTotal = 0;

                // Get all inventory items for this company at once to reduce DB calls.
                var companyInventory = await _context.Inventories
                    .Where(i => i.UserId == company.Id)
                    .ToDictionaryAsync(i => i.MedicineId, i => i);

                foreach (var order in group)
                {
                    decimal orderTotal = 0;
                    foreach (var item in order.Items)
                    {
                        if (companyInventory.TryGetValue(item.MedicineId, out var inventoryItem))
                        {
                            orderTotal += inventoryItem.Price * item.Quantity;
                        }
                    }
                    companyTotal += orderTotal;
                }

                indebtednessSummaries.Add(new IndebtednessSummaryViewModel
                {
                    CompanyId = company.Id,
                    CompanyName = company.DisplayName,
                    TotalAmount = companyTotal
                });

                totalIndebtedness += companyTotal;
            }

            // 4. Prepare the final view model.
            var viewModel = new IndebtednessViewModel
            {
                IndebtednessSummaries = indebtednessSummaries.OrderByDescending(s => s.TotalAmount).ToList(),
                TotalIndebtedness = totalIndebtedness,
                TotalPaid = 0, // Placeholder for now.
                CompaniesCount = indebtednessSummaries.Count
            };

            return View(viewModel);
        }
    }
}