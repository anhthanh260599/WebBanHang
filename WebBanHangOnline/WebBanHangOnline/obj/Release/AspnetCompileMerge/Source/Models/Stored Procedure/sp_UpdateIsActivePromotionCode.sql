CREATE PROCEDURE [dbo].[sp_UpdateIsActivePromotionCode]
AS
BEGIN
  SET NOCOUNT ON;
    DECLARE @CurrentTime DATETIME = GETDATE();

	-- Xử lý cho trong khoảng thời gian
    UPDATE tb_Promotion
    SET IsActive = 1
    WHERE @CurrentTime >= DateFrom
	AND @CurrentTime <= DateTo;

	-- Xử lý cho ngoài khoảng thời gian
	UPDATE tb_Promotion
    SET IsActive = 0
    WHERE @CurrentTime < DateFrom
	OR @CurrentTime > DateTo;

	-- Xử lý cho trong khoảng thời gian và Quantity < 1
	UPDATE tb_Promotion
    SET IsActive = 0
    WHERE Quantity < 1
	AND(@CurrentTime >= DateFrom
	AND @CurrentTime <= DateTo);
END;