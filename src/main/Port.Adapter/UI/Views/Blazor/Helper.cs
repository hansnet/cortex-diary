﻿using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Notifications;
using ei8.Cortex.Diary.Port.Adapter.UI.Common;
using ei8.Cortex.Library.Common;
using neurUL.Cortex.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
{
    public static class Helper
    {
        internal static void ReinitializeOption(Action<ContextMenuOption> optionSetter)
        {
            optionSetter(ContextMenuOption.NotSet);
            optionSetter(ContextMenuOption.New);
        }

        internal static void FillUIIds(this IEnumerable<UINeuron> neurons, UINeuron central)
        {
            neurons.ToList().ForEach(un =>
            {
                un.UIId = Guid.NewGuid().GetHashCode();
                un.CentralUIId = central != null ? central.UIId : int.MinValue;
            });
        }

        internal static async Task CreateRelativeCore(INeuronApplicationService neuronApplicationService, ITerminalApplicationService terminalApplicationService, string avatarUrl, string regionId, string targetNeuronId, RelativeType relativeType, string tag, NeurotransmitterEffect effect, float strength)
        {
            var presynapticNeuronId = string.Empty;
            var postsynapticNeuronId = string.Empty;
            var newNeuronId = string.Empty;

            if (relativeType == RelativeType.Presynaptic)
            {
                newNeuronId = presynapticNeuronId = Guid.NewGuid().ToString();
                postsynapticNeuronId = targetNeuronId;
            }
            else if (relativeType == RelativeType.Postsynaptic)
            {
                presynapticNeuronId = targetNeuronId;
                newNeuronId = postsynapticNeuronId = Guid.NewGuid().ToString();
            }

            await neuronApplicationService.CreateNeuron(
                avatarUrl,
                newNeuronId,
                tag,
                regionId
            );
            await terminalApplicationService.CreateTerminal(
                avatarUrl,
                Guid.NewGuid().ToString(),
                presynapticNeuronId,
                postsynapticNeuronId,
                effect,
                strength
                );
        }

        public static async Task LinkRelativeCore(ITerminalApplicationService terminalApplicationService, string avatarUrl, string targetNeuronId, RelativeType relativeType, IEnumerable<UINeuron> candidates, NeurotransmitterEffect effect, float strength)
        {
            foreach (var n in candidates)
            {
                await terminalApplicationService.CreateTerminal(
                    avatarUrl,
                    Guid.NewGuid().ToString(),
                    relativeType == RelativeType.Presynaptic ? n.Id : targetNeuronId,
                    relativeType == RelativeType.Presynaptic ? targetNeuronId : n.Id,
                    effect,
                    strength
                    );
            }
        }

        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        // TODO: internal async static Task<bool> ChangeNeuronTag(string tag, INeuronApplicationService neuronApplicationService, IStatusService statusService, string avatarUrl, string targetNeuronId, int expectedVersion, string bearerToken)
        //{
        //    bool result = false;

        //    await Neurons.Helper.SetStatusOnComplete(async () =>
        //    {
        //        await neuronApplicationService.ChangeNeuronTag(
        //                avatarUrl,
        //                targetNeuronId,
        //                tag,
        //                expectedVersion,
        //                bearerToken
        //            );

        //        return result = true;
        //    },
        //        "Neuron Tag changed successfully.",
        //        statusService
        //    );
        //    return result;
        //}
    }
}
