# Pipelines - CI/CD Platform Frontend

一个现代化的 CI/CD 平台前端界面，基于 **Vite + TypeScript** 构建，采用极简设计风格。

## 🚀 技术栈

- **⚡ Vite** - 超快的前端构建工具
- **🔷 TypeScript** - 类型安全的 JavaScript
- **⚛️ React 18** - 现代化 React 框架
- **🎨 Tailwind CSS** - 实用优先的 CSS 框架
- **🧭 React Router 6** - 客户端路由
- **📦 Heroicons** - 美观的 SVG 图标库
- **📅 Moment.js** - 时间处理库
- **🌐 Axios** - HTTP 客户端

## 🎯 核心功能

### 📊 仪表板 (Dashboard)
- 构建统计概览
- 成功率指标
- 最近构建列表
- 实时状态监控

### 📁 存储库管理 (Repositories)
- 存储库列表和搜索
- 状态过滤 (激活/未激活)
- 可见性过滤 (公开/私有)
- 构建历史查看

### 🔧 构建详情 (Build Details)
- 构建步骤可视化
- 实时日志查看
- 构建信息详情
- 操作控制 (取消/重启)

### ⚙️ 设置页面 (Settings)
- 通用设置 (语言、时区、主题)
- 通知配置 (邮件、浏览器通知)
- 安全设置 (双因素认证、会话管理)
- 服务器配置 (URL、超时、并发限制)

## 🎨 设计特色

### 🎪 极简美学
- 简洁的线条和充足的留白
- 精心调配的灰度色彩系统
- 微妙的状态颜色提示
- 现代化的排版设计

### 📱 响应式布局
- 桌面端：顶部水平导航
- 移动端：右滑菜单
- 自适应组件和网格布局
- 触控友好的交互设计

### 🚀 性能优化
- TypeScript 类型安全
- Vite 热重载开发体验
- 组件级代码分割
- 优化的打包输出

## 🛠️ 开发指南

### 📦 安装依赖
```bash
npm install
```

### 🏃‍♂️ 启动开发服务器
```bash
npm run dev
# 访问 http://localhost:3000
```

### 🔧 类型检查
```bash
npm run type-check
```

### 🏗️ 构建生产版本
```bash
npm run build
```

### 👀 预览生产构建
```bash
npm run preview
```

### 🔍 代码检查
```bash
npm run lint
```

## 📁 项目结构

```
ui/
├── src/
│   ├── components/          # 可复用组件
│   │   ├── Layout/         # 布局组件
│   │   ├── BuildStatus/    # 构建状态组件
│   │   ├── BuildCard/      # 构建卡片组件
│   │   └── RepositoryCard/ # 存储库卡片组件
│   ├── pages/              # 页面组件
│   │   ├── Dashboard.tsx   # 仪表板
│   │   ├── Repositories.tsx # 存储库列表
│   │   ├── Repository.tsx  # 存储库详情
│   │   ├── Build.tsx       # 构建详情
│   │   └── Settings.tsx    # 设置页面
│   ├── types/              # TypeScript 类型定义
│   │   └── index.ts        # 通用类型和接口
│   ├── App.tsx             # 根应用组件
│   ├── main.tsx            # 应用入口点
│   ├── index.css           # 全局样式
│   └── App.css             # 应用样式
├── public/                 # 静态资源
├── index.html              # HTML 模板
├── vite.config.ts          # Vite 配置
├── tsconfig.json           # TypeScript 配置
├── tailwind.config.js      # Tailwind CSS 配置
└── package.json            # 项目配置
```

## 🔧 配置说明

### Vite 配置
- 开发服务器端口：3000
- API 代理：`/api` → `http://localhost:8080`
- 构建输出：`dist/` 目录

### TypeScript 配置
- 严格模式启用
- 路径映射支持 (`@/*` → `src/*`)
- React JSX 支持
- ES2020 目标

### Tailwind CSS
- 自定义颜色系统
- 组件类定义
- 响应式设计支持
- 自定义字体 (Inter + JetBrains Mono)

## 🎯 未来规划

- [ ] 🔌 真实 API 集成
- [ ] 🧪 单元测试覆盖
- [ ] 📊 性能监控
- [ ] 🌐 国际化支持
- [ ] 🎨 深色主题完善
- [ ] 📱 PWA 支持

---

**🚀 现在就开始使用 `npm run dev` 体验全新的 TypeScript + Vite 开发环境！** 