﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LiquidToFilter : Task {

    public enum Conditions { AddedLiquid }
    
    private FilterPart pumpFilter;
    LiquidType liquidType;

    private readonly int REQUIRED_AMOUNT;

    public LiquidToFilter(string description, int amount, LiquidType liquid, TaskType taskType) : base(TaskType.WetFilter, true, true) {
        this.description = description;
        liquidType = liquid;
        REQUIRED_AMOUNT = amount;
        TaskType = taskType;
        SetCheckAll(true);
        AddConditions((int[])Enum.GetValues(typeof(Conditions)));
        SubscribeEvent(OnFilterWet, EventType.TransferLiquidToBottle);
    }

    private void OnFilterWet(CallbackData data) {
        LiquidContainer container = data.DataObject as LiquidContainer;
        if (container.GeneralItem is FilterPart filter && filter.ObjectType == ObjectType.PumpFilterTank) {
            pumpFilter = filter;
            if (filter.Container.Amount >= REQUIRED_AMOUNT) {
                Logger.Print("Enabling condotions");
                EnableCondition(Conditions.AddedLiquid);
                CheckMistakes();
                CompleteTask();
            }
        }
        /*if (Started) {
            if (container.GeneralItem is FilterPart filter && filter.ObjectType == ObjectType.PumpFilterTank) {
                pumpFilter = filter;
                if (filter.Container.Amount >= REQUIRED_AMOUNT) {
                    Logger.Print("Enabling condotions");
                    EnableCondition(Conditions.AddedLiquid);
                    CheckMistakes();
                    CompleteTask();
                }
            }
        }*/
    }

    private void CheckMistakes() {
        if (pumpFilter.Container.LiquidType != liquidType && liquidType == LiquidType.Peptonwater) {
            CreateGeneralMistake("Et lisännyt peptonivettä filtteriin", 1);
        }
        if (pumpFilter.Container.LiquidType != liquidType && liquidType == LiquidType.Medicine) {
            CreateGeneralMistake("Et lisännyt lääkettä filtteriin", 1);
        }
        if (pumpFilter.Container.Amount > REQUIRED_AMOUNT) {
            CreateTaskMistake("Filtterissä on liikaa nestettä", 1);
        }
        if (pumpFilter.Container.Impure) {
            CreateTaskMistake("Filtterin neste on sekoittunut", 1);
        }
    }

    protected override void OnTaskComplete() {
        // juu
    }

    public override void CompleteTask() {
        base.CompleteTask();
        if (Completed) {
            Popup("Hienosti kostutettu!", MsgType.Done);
        }
    }
}