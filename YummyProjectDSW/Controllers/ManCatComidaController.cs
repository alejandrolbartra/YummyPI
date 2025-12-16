using Microsoft.AspNetCore.Mvc;
using System.Data;
using yummyApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace yummyApp.Controllers
{
    public class ManCatComidaController : Controller
    {

        private readonly IConfiguration _config;
        public ManCatComidaController(IConfiguration config)
        {
            _config = config;
        }

        string mergeCatComida(ManComida cat)
        {
            string mensaje = "";

            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_merge_catComida", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", cat.idCategoriaComida);
                    cmd.Parameters.AddWithValue("@nom", cat.nombreCategoriaComida);
                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha adicionado o alterado {i} categoria-comida";
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        IEnumerable<ManComida> listCatComida()
        {
            List<ManComida> temporal = new List<ManComida>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("usp_catComida", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new ManComida()
                    {
                        idCategoriaComida = dr.GetInt32(0),
                        nombreCategoriaComida = dr.GetString(1),
                    });
                }
                dr.Close();
            }
            return temporal;
        }

        public async Task<IActionResult> ListadoGeneralCatComida(int numreg = 10, int page = 0)
        {
            var temporal = listCatComida();

            //  Paginación
            int total = temporal.Count();
            int pags = total % numreg == 0 ? total / numreg : total / numreg + 1;

            ViewBag.page = page;
            ViewBag.pags = pags;
            ViewBag.numreg = numreg;

            var resultado = temporal.Skip(page * numreg).Take(numreg);

            return View(await Task.Run(() => resultado));
        }

        ManComida Buscar(int id)
        {
            return listCatComida().Where(v => v.idCategoriaComida == id).FirstOrDefault();
        }

        public async Task<ActionResult> Create()
        {
            return View(await Task.Run(() => new ManComida()));
            //return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ManComida cat)
        {
            if (!ModelState.IsValid)
            {
                return View(await Task.Run(() => cat));
            }

            cat.idCategoriaComida = 0;
            ViewBag.mensaje = mergeCatComida(cat);
            return View(await Task.Run(() => cat));
            //return View();
        }

        public async Task<ActionResult> Edit(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Index");

            ManComida cat = Buscar(id.Value);
            return View(await Task.Run(() => cat));
            //return View();
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ManComida cat)
        {
            if (!ModelState.IsValid)
            {
                return View(await Task.Run(() => cat));
            }
            ViewBag.mensaje = mergeCatComida(cat);
            return View(await Task.Run(() => cat));
            //return View();
        }

        public async Task<ActionResult> Details(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Index");

            ManComida reg = Buscar(id.Value);
            return View(await Task.Run(() => reg));
        }

        [HttpPost, ActionName("Delete")]

        public async Task<ActionResult> Delete(int id)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                SqlCommand cmd = new SqlCommand("usp_desactivar_catComida", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            return RedirectToAction("ListadoGeneralCatComida");
        }

    }
}
