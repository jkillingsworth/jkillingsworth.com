module Compile

open System

//------------------------------------------------------------------------------------------------

type private LinqExp = System.Linq.Expressions.Expression
type private MathExp = MathNet.Symbolics.Expression

//------------------------------------------------------------------------------------------------

let expression symbols ex =

    let exParameter = LinqExp.Parameter(typeof<float[]>)

    let rec convert = function
        | MathExp.Approximation r
            -> LinqExp.Constant(r.RealValue) :> LinqExp
        | MathExp.Number n
            -> LinqExp.Constant(float n) :> LinqExp
        | MathExp.Identifier s
            ->
            let i = symbols |> Array.findIndex (fun x -> x = s)
            let exIndex = LinqExp.Constant(i)
            let exArray = LinqExp.ArrayAccess(exParameter, exIndex)
            exArray :> LinqExp
        | MathExp.Sum xs
            ->
            let f a b = LinqExp.Add(a, b) :> LinqExp
            xs |> List.map convert |> List.reduce f
        | MathExp.Product xs
            ->
            let f a b = LinqExp.Multiply(a, b) :> LinqExp
            xs |> List.map convert |> List.reduce f
        | MathExp.Power (x, n)
            -> LinqExp.Power(convert x, convert n) :> LinqExp
        | _
            -> failwith "Unexpected expression."

    let ex = convert ex
    let ex = LinqExp.Lambda(ex, exParameter)
    let dg = ex.Compile()
    let dx = dg :?> Func<float[], float>
    let fn xs = dx.Invoke(xs)
    fn
