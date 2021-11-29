// Learn more about F# at http://fsharp.org

open System

type Customer = Customer of char

type Recurrence =
    | MonthDay of int
    | WeekDays of List<DayOfWeek>
    | EveryDay
    | Never

let filterKeysByValue<'K,'T when 'K: comparison> (p: ('T -> bool)) (m: Map<'K,'T>) =
    m 
    |> Map.fold (fun ks k t -> if (p t) then k::ks else ks) []

let dateSeq numberOfdays (starting: DateTime) =
    float 
    |> Seq.init numberOfdays
    |> Seq.map starting.AddDays

let recurrenceDatePred recurrence (date: DateTime) =
    match recurrence with
    | MonthDay md when md >= 1 && md <= 28 -> md = date.Day
    | MonthDay _ -> false
    | WeekDays wds -> wds |> List.exists (fun wd -> wd = date.DayOfWeek)
    | EveryDay -> true
    | Never -> false

let filterRecurrenceByDate recurrenceMap (date: DateTime) =
    recurrenceMap
    |> filterKeysByValue (fun recurrence -> date |> recurrenceDatePred recurrence)

let createPeriodicReport valuesForDate totalDays (starting: DateTime) =
    starting
    |> dateSeq totalDays 
    |> Seq.map (fun date -> (date, date |> valuesForDate))
    |> Map.ofSeq

[<EntryPoint>]
let main _ =
    let printer (m: Map<'K, 'T>) =
        for kv in m do
            printfn $"{kv.Key}: {kv.Value}"

    let customerContactRecurrences =
        Map.empty.
            Add(Customer 'A', EveryDay).
            Add(Customer 'B', MonthDay 10).
            Add(Customer 'C', WeekDays [ DayOfWeek.Tuesday; DayOfWeek.Friday ])

    let customerContactRecurrencePeriodicReport =
        customerContactRecurrences
        |> filterRecurrenceByDate
        |> createPeriodicReport

    let customerContactRecurrence90DayReport =
        90
        |> customerContactRecurrencePeriodicReport

    let customerContactRecurrence90DayReportFromDate date =
        date
        |> customerContactRecurrence90DayReport
        |> Map.map (fun _ cs -> cs |> List.sortBy (fun c -> c))

    new DateTime(2018, 4, 1)
    |> customerContactRecurrence90DayReportFromDate
    |> printer
    0 // return an integer exit code