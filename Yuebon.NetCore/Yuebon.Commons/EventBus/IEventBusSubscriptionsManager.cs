﻿
using Yuebon.Commons.EventBus.Abstractions;
using Yuebon.Commons.EventBus.Events;
using static Yuebon.Commons.EventBus.InMemoryEventBusSubscriptionsManager;

namespace Yuebon.Commons.EventBus;

/// <summary>
/// 事件总线订阅管理器
/// 接口
/// </summary>
public interface IEventBusSubscriptionsManager
{
    /// <summary>
    /// 是否为空
    /// </summary>
    bool IsEmpty { get; }
    /// <summary>
    /// 移除事件
    /// </summary>
    event EventHandler<string> OnEventRemoved;
    /// <summary>
    /// 添加订阅
    /// </summary>
    /// <typeparam name="TH"></typeparam>
    /// <param name="eventName"></param>
    void AddDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler;
    /// <summary>
    /// 添加订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TH"></typeparam>
    void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;
    /// <summary>
    /// 移除订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TH"></typeparam>
    void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    /// <summary>
    /// 移除订阅
    /// </summary>
    /// <typeparam name="TH"></typeparam>
    /// <param name="eventName">事件名称</param>
    void RemoveDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler;
    /// <summary>
    /// 是否存在订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
    /// <summary>
    /// 通过事件名称判断是否存在订阅
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns></returns>
    bool HasSubscriptionsForEvent(string eventName);
    /// <summary>
    /// 通过事件名称获取事件类型
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns></returns>
    Type GetEventTypeByName(string eventName);
    /// <summary>
    /// 清除
    /// </summary>
    void Clear();
    /// <summary>
    /// 获取事件处理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
    /// <summary>
    /// 获取事件处理器
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns></returns>
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    string GetEventKey<T>();
}
