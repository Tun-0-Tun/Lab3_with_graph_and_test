#include "pch.h"
#include <mkl.h>
#include <mkl_df_defines.h>

#include <iostream>
#include <vector>

enum class ErrorCode {NO, INIT_ERROR, CHECK_ERROR, SOLVE_ERROR, JACOBI_ERROR, GET_ERROR, DELETE_ERROR, RCI_ERROR };

struct SplineParametrs
{
    double* nonUniformGridPoints;
    double* nonUniformGridValues;
    int nonUniformGridLength;
	MKL_INT splineOrder;
    MKL_INT boundaryType;
    MKL_INT splineType;
    double* borders;
    int lengthNodes;
    double* splineCoefficients;
	int res;
   
};

// подсчёт значений сплайна на неравномерной сетке
void NonUniformSpline(MKL_INT* lengthX, MKL_INT* lengthNodes, double* valuesOnUniformGrid, double* f, void* userData)
{
	// void * - единственный добустимый вариант передачи остальных параметов
    SplineParametrs params = *((SplineParametrs*)userData);
    int status = -1;
    DFTaskPtr task;

	// строим равномерный сплайн
    status = dfdNewTask1D(
		&task,params.lengthNodes, params.borders,
        DF_UNIFORM_PARTITION,
        1, valuesOnUniformGrid,
        DF_NO_HINT);

    if (status != DF_STATUS_OK) throw "Error dfdNewTask1D";

	// записываем коэффициенты в splineCoefficients, DF_NO_HINT - для оптимизации
    status = dfdEditPPSpline1D(task,
        params.splineOrder, params.splineType,
        params.boundaryType, NULL,
        DF_NO_IC, NULL,
        params.splineCoefficients, DF_NO_HINT);

    if (status != DF_STATUS_OK) throw "Error dfdEditPPSpline1D";

	// конструирование сплайна
    status = dfdConstruct1D(task,
        DF_PP_SPLINE,
        DF_METHOD_STD);

    if (status != DF_STATUS_OK) throw "Error dfdConstruct1D";

	// флаг порядка подчёта производных
    int nDorder = 1;
    MKL_INT dOrder[] = { 1 };
    double* splineWithDerivatives = new double[*lengthX];

	// подсчёт значений функций на неравномерной сетке
    status = dfdInterpolate1D(task,
        DF_INTERP, DF_METHOD_PP,
        params.nonUniformGridLength, params.nonUniformGridPoints,
        DF_NON_UNIFORM_PARTITION,
        nDorder, dOrder,
        NULL,
        splineWithDerivatives, 
		DF_NO_HINT, NULL);

    if (status != DF_STATUS_OK) throw "Error dfdInterpolate1D";


    for (int i = 0; i < params.nonUniformGridLength; ++i) {
		// считаем квадрат невязки
		if (params.res){
			f[i] = std::pow((splineWithDerivatives[i] - params.nonUniformGridValues[i]), 2);	
		} else { 
			f[i] = splineWithDerivatives[i];
		}

	}
    status = dfDeleteTask(&task);
    if (status != DF_STATUS_OK) std::cerr << "Error dfDeleteTask" << std::endl;

    delete[] splineWithDerivatives;
}

extern "C" _declspec(dllexport)
void SplineInterpolation(
    int nonUniformNodesCnt,
    double* nonUniformNodes,
    int vectorDimension,
    double* vectorValues,
    int numUniformNodes,
    double* initialApproximation,
    double* splineValues,
    int* stopReason,
    int maxIterations,
    int* actualNumberOfIterations,
    double* addGrid,
    double* addSplineData,
    int addSize)
{
    MKL_INT splineOrder = DF_PP_CUBIC; // кубический сплайн
    MKL_INT splineType = DF_PP_NATURAL; // простой сплай
    MKL_INT bcType = DF_BC_FREE_END; // типы граничных условий
   
    double* splineCoefficients = new double[1 * (numUniformNodes - 1) * splineOrder];
    int status = -1;
    double jacobianEps = 1.0E-8;

    double initialRes = 0;
    double finalRes = 0;
    double rho = 10;

    MKL_INT maxOuterIterations = maxIterations;
    MKL_INT maxInnerIterations = 100;

    MKL_INT doneIterations = 0;
    MKL_INT terminationReason;
    MKL_INT checkDataInfo[4];
    

    const double eps[] = {
        1.0E-12,
        1.0E-12,
        1.0E-12,
        1.0E-12,
        1.0E-12,
        1.0E-12
    };

   

    
    
    ErrorCode error = ErrorCode(ErrorCode::NO);

    _TRNSP_HANDLE_t handle = NULL;
    double* resVector = NULL, * jacobianMatrix = NULL;

    try
    {
        double nonUniformBoundaries[2] = { nonUniformNodes[0], nonUniformNodes[nonUniformNodesCnt - 1] };

        resVector = new double[nonUniformNodesCnt];
        jacobianMatrix = new double[nonUniformNodesCnt * numUniformNodes];

        MKL_INT ret = dtrnlsp_init(&handle, &numUniformNodes, &nonUniformNodesCnt, initialApproximation, eps, &maxOuterIterations, &maxInnerIterations, &rho);
        if (ret != TR_SUCCESS) throw (ErrorCode(ErrorCode::INIT_ERROR));

        ret = dtrnlsp_check(&handle, &numUniformNodes, &nonUniformNodesCnt, jacobianMatrix, resVector, eps, checkDataInfo);
        if (ret != TR_SUCCESS) throw (ErrorCode(ErrorCode::CHECK_ERROR));

        MKL_INT rciRequest = 0;

        SplineParametrs SplineDesignInformation;
        SplineDesignInformation.nonUniformGridLength = nonUniformNodesCnt;
        SplineDesignInformation.nonUniformGridPoints = nonUniformNodes;
        SplineDesignInformation.nonUniformGridValues = vectorValues;
        SplineDesignInformation.lengthNodes = numUniformNodes;
        SplineDesignInformation.borders = nonUniformBoundaries;
        SplineDesignInformation.splineCoefficients = splineCoefficients;
        SplineDesignInformation.splineOrder = splineOrder;
        SplineDesignInformation.splineType = splineType;
        SplineDesignInformation.boundaryType = bcType;
		SplineDesignInformation.res = true;

        bool skipSplineConstruction = false;
        while (true)
        {
            if (!skipSplineConstruction) {
                skipSplineConstruction = true;
            }

            ret = dtrnlsp_solve(&handle, resVector, jacobianMatrix, &rciRequest);
            if (ret != TR_SUCCESS) throw (ErrorCode(ErrorCode::SOLVE_ERROR));

            if (rciRequest == 0) continue;
            else if (rciRequest == 1) NonUniformSpline(&nonUniformNodesCnt, &numUniformNodes, initialApproximation, resVector, static_cast<void*>(&SplineDesignInformation));
            else if (rciRequest == 2)
            {
                ret = djacobix(NonUniformSpline, &numUniformNodes, &nonUniformNodesCnt, jacobianMatrix, initialApproximation, &jacobianEps, static_cast<void*>(&SplineDesignInformation));
                if (ret != TR_SUCCESS) throw (ErrorCode(ErrorCode::JACOBI_ERROR));
            }
            else if (rciRequest >= -6 && rciRequest <= -1) break;
            else throw (ErrorCode(ErrorCode::RCI_ERROR));
        }

        ret = dtrnlsp_get(&handle, &doneIterations, &terminationReason, &initialRes, &finalRes);
        if (ret != TR_SUCCESS) throw (ErrorCode(ErrorCode::GET_ERROR));

        ret = dtrnlsp_delete(&handle);
        if (ret != TR_SUCCESS) throw (ErrorCode(ErrorCode::DELETE_ERROR));

        double* values = new double[nonUniformNodesCnt];
		SplineDesignInformation.res = false;
        NonUniformSpline(&nonUniformNodesCnt, &numUniformNodes, initialApproximation, values, static_cast<void*>(&SplineDesignInformation));

        for (int i = 0; i < nonUniformNodesCnt; ++i)
        {
            splineValues[i] = values[i];
        }

        *stopReason = terminationReason;
        *actualNumberOfIterations = doneIterations;

        delete[] values;

        SplineDesignInformation.nonUniformGridPoints = addGrid;
        SplineDesignInformation.nonUniformGridLength = addSize;
        NonUniformSpline(&addSize, &numUniformNodes, initialApproximation, addSplineData, static_cast<void*>(&SplineDesignInformation));

    }
    catch (ErrorCode _error) { error = _error; }
    catch (const char* str)
    {
        std::cerr << std::string(str) << std::endl;
        std::cout << std::string(str) << std::endl;
    }

    if (resVector != NULL) delete[] resVector;
    if (jacobianMatrix != NULL) delete[] jacobianMatrix;

    delete[] splineCoefficients;
}
