using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using yummyApp.Models;

namespace yummyApp.Controllers
{

    public class ManCatOrigenController : Controller
    {
              
        private readonly IConfiguration _config;
        public ManCatOrigenController(IConfiguration config)
        {
            _config = config;
        }

        string mergeCatOrigen(CategoriaOrigen cat)
        {
            string mensaje = "";

            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_merge_catOrigen", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", cat.idCategoriaOrigen);
                    cmd.Parameters.AddWithValue("@nom", cat.nombreCategoriaOrigen);
                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha adicionado o alterado {i} categoria-origen";
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        IEnumerable<CategoriaOrigen> listCatOrigen()
        {
            List<CategoriaOrigen> temporal = new List<CategoriaOrigen>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("usp_catOrigen", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new CategoriaOrigen()
                    {
                        idCategoriaOrigen = dr.GetInt32(0),
                        nombreCategoriaOrigen = dr.GetString(1),
                    });
                }
                dr.Close();
            }
            return temporal;
        }

        public async Task<IActionResult> ListadoGeneralCatOrigen( int numreg = 10, int page = 0)
        {
            var temporal = listCatOrigen();

            //  Paginación
            int total = temporal.Count();
            int pags = total % numreg == 0 ? total / numreg : total / numreg + 1;

            ViewBag.page = page;
            ViewBag.pags = pags;
            ViewBag.numreg = numreg;

            var resultado = temporal.Skip(page * numreg).Take(numreg);

            return View(await Task.Run(() => resultado));
        }

        CategoriaOrigen Buscar(int id)
        {
            return listCatOrigen().Where(v => v.idCategoriaOrigen == id).FirstOrDefault();
        }

        

        public async Task<ActionResult> Create()
        {
            return View(await Task.Run(() => new CategoriaOrigen()));
            //return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CategoriaOrigen cat)
        {
            if (!ModelState.IsValid)
            {
                return View(await Task.Run(() => cat));
            }

            cat.idCategoriaOrigen = 0;
            ViewBag.mensaje = mergeCatOrigen(cat);
            return View(await Task.Run(() => cat));
            //return View();
        }

        public async Task<ActionResult> Edit(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Index");

            CategoriaOrigen cat = Buscar(id.Value);
            return View(await Task.Run(() => cat));
            //return View();
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CategoriaOrigen cat)
        {
            if (!ModelState.IsValid)
            {
               return View(await Task.Run(() => cat));
            }
            ViewBag.mensaje = mergeCatOrigen(cat);
            return View(await Task.Run(() => cat));
            //return View();
        }

        public async Task<ActionResult> Details(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Index");

            CategoriaOrigen reg = Buscar(id.Value);
            return View(await Task.Run(() => reg));
        }

        [HttpPost, ActionName("Delete")]

        public async Task<ActionResult> Delete(int id)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                SqlCommand cmd = new SqlCommand("usp_desactivar_catOrigen", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            return RedirectToAction("ListadoGeneralCatOrigen");
        }

    }
}
