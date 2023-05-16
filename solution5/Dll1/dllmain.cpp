#include "pch.h"
#include "mkl.h" // пришлось добавлять путь к MKL через свойства, не уверен будет ли рабоать при обычном копировании с github

// df?NewTask1D - создание задачи
// 
// df?EditPPSpline1D - Настройка параметров задачи − выбор тип сплайна и граничных
// условий для сплайна.
// 
// df?Construct1D - Создание сплайна.
// 
// df?Interpolate1D - Вычисление значений сплайна и производных.
// 
// df?Integrate1D - Вычисление интегралов на основе сплайнов.
// 
// dfDeleteTask - Вызов деструктора задачи, который освобождает ресурсы.
// 
// 
//
extern "C" _declspec(dllexport) // доступна для эскпортирования

double func(int NumSplines, int NumNodes, const double Points[], const double Values[], bool isUniform, double firstdiff[], double coefficients[], double res[], double integral[]) {
	DFTaskPtr task; // создание задачи
	MKL_INT how_interpolate[] = { 1, 1, 1 };
	double leftl[] = { Points[0] };
	double rightl[] = { Points[NumNodes - 1] };
	double PointsStartEnd[] = { Points[0], Points[NumNodes - 1] }; // обязательно массив
	// передаем данные, для которых выполняется сплайн интерполяция
	if (isUniform) { // проверка на равномерное разибение
		dfdNewTask1D(&task, NumNodes, PointsStartEnd, DF_UNIFORM_PARTITION, 1, Values, DF_MATRIX_STORAGE_ROWS);
		// 1 - число функций, которео будет интерполироваться
	}
	else {
		dfdNewTask1D(&task, NumNodes, Points, DF_NON_UNIFORM_PARTITION, 1, Values, DF_MATRIX_STORAGE_ROWS);
	}
	// выполняем конфигурацию задачи, выбираем тип сплайна и граничные условия
	dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_1ST_LEFT_DER | DF_BC_1ST_RIGHT_DER, firstdiff, DF_NO_IC, NULL, coefficients, DF_NO_HINT);
	// построение сплайна, вычисление производных и интеграла 
	dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
	// выполняем итерполяцию 
	dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, NumSplines, PointsStartEnd, DF_UNIFORM_PARTITION, 3, how_interpolate, NULL, res, DF_MATRIX_STORAGE_ROWS, NULL);
	// Вычисление интегралов от интерполяционного сплайна
	dfdIntegrate1D(task, DF_METHOD_PP, 1, leftl, DF_UNIFORM_PARTITION, rightl, DF_UNIFORM_PARTITION, NULL, NULL, integral, DF_MATRIX_STORAGE_ROWS);
	dfDeleteTask(&task);
	return 1;
}

