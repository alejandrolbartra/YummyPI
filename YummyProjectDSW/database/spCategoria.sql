

create or alter proc usp_catOrigen
as
select * from CategoriaOrigen
where estado = 1
go

--

create or alter proc usp_merge_catOrigen

    @id int,
    @nom varchar(255)
as
begin
    set nocount off;
    if exists (select 1 from CategoriaOrigen where idCategoriaOrigen = @id)
    begin
        update CategoriaOrigen
        set nombreCategoriaOrigen = @nom
        where idCategoriaOrigen = @id;
    end
    else
    begin
        insert into CategoriaOrigen (nombreCategoriaOrigen, estado)
        values (@nom, 1);
    end
end

go


CREATE OR ALTER PROCEDURE usp_merge_producto
    @id INT,
    @nom VARCHAR(255),
    @prec DECIMAL(10,2),
    @stock INT,
    @id_cat_or INT,
    @id_cat_com INT
AS
BEGIN
    SET NOCOUNT OFF;
    IF EXISTS (SELECT 1 FROM Producto WHERE idProducto = @id)
    BEGIN
        UPDATE Producto
        SET
            nombreProducto = @nom,
            precioProd = @prec,
            stockProd = @stock,
            idCategoriaOrigen = @id_cat_or,
            idCategoriaComida = @id_cat_com
        WHERE idProducto = @id;
    END
    ELSE
    BEGIN
        INSERT INTO Producto (nombreProducto, precioProd, stockProd, idCategoriaOrigen, idCategoriaComida)
        VALUES (@nom, @prec, @stock, @id_cat_or, @id_cat_com);
    END
END
GO

--

create or alter proc usp_desactivar_catOrigen
@id int
as
update CategoriaOrigen
set estado = 0
where idCategoriaOrigen = @id
go
